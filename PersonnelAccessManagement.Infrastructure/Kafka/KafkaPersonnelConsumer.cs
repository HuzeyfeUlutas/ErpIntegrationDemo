using System.Text.Json;
using Confluent.Kafka;
using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersonnelAccessManagement.Application.Common.Constants;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Domain.Entities;
using PersonnelAccessManagement.Domain.Enums;
using PersonnelAccessManagement.Domain.Events;

namespace PersonnelAccessManagement.Infrastructure.Kafka;

public sealed class KafkaPersonnelConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly KafkaConsumerOptions _options;
    private readonly ILogger<KafkaPersonnelConsumer> _logger;

    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    private static readonly Dictionary<PersonnelEventType, string> TopicMap = new()
    {
        [PersonnelEventType.Hired] = CapTopics.Hired,
        [PersonnelEventType.Terminated] = CapTopics.Terminated,
        [PersonnelEventType.PositionChanged] = CapTopics.PositionChanged,
    };

    public KafkaPersonnelConsumer(
        IServiceScopeFactory scopeFactory,
        IOptions<KafkaConsumerOptions> options,
        ILogger<KafkaPersonnelConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Kafka consumer starting — Topic: {Topic}, GroupId: {GroupId}, Servers: {Servers}",
            _options.Topic, _options.GroupId, _options.BootstrapServers);

        await Task.Run(() => ConsumeLoop(stoppingToken), stoppingToken);
    }

    private void ConsumeLoop(CancellationToken ct)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            GroupId = _options.GroupId,
            AutoOffsetReset = Enum.Parse<AutoOffsetReset>(_options.AutoOffsetReset, ignoreCase: true),
            EnableAutoCommit = _options.EnableAutoCommit,
            EnableAutoOffsetStore = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) =>
                _logger.LogError("Kafka error: {Reason} (IsFatal: {IsFatal})", e.Reason, e.IsFatal))
            .SetPartitionsAssignedHandler((_, partitions) =>
                _logger.LogInformation("Partitions assigned: {Partitions}",
                    string.Join(", ", partitions)))
            .SetPartitionsRevokedHandler((_, partitions) =>
                _logger.LogInformation("Partitions revoked: {Partitions}",
                    string.Join(", ", partitions)))
            .Build();

        consumer.Subscribe(_options.Topic);

        try
        {
            while (!ct.IsCancellationRequested)
            {
                ConsumeResult<string, string>? result = null;
                try
                {
                    result = consumer.Consume(ct);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka consume error on topic {Topic}", _options.Topic);
                    continue;
                }

                if (result?.Message?.Value is null)
                    continue;

                ProcessMessage(result, consumer, ct).GetAwaiter().GetResult();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer stopping (graceful shutdown).");
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("Kafka consumer closed.");
        }
    }

    private async Task ProcessMessage(
        ConsumeResult<string, string> result,
        IConsumer<string, string> consumer,
        CancellationToken ct)
    {
        // ── 1) DESERIALIZE ──────────────────────────────────────
        PersonnelLifecycleIntegrationEvent? evt;
        try
        {
            evt = JsonSerializer.Deserialize<PersonnelLifecycleIntegrationEvent>(result.Message.Value, JsonOpts);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex,
                "DESERIALIZE ERROR — Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, Key: {Key}, Value: {Value}",
                result.Topic, result.Partition.Value, result.Offset.Value,
                result.Message.Key, result.Message.Value);

            await LogToDatabase(result, ex.Message, ex.StackTrace, "POISON", ct);
            CommitOffset(consumer, result);
            return;
        }

        if (evt is null)
        {
            _logger.LogWarning(
                "Deserialized to null — Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
                result.Topic, result.Partition.Value, result.Offset.Value);

            await LogToDatabase(result, "Deserialized to null", null, "POISON", ct);
            CommitOffset(consumer, result);
            return;
        }

        // ── 2) CAP TOPIC BUL ───────────────────────────────────
        if (!TopicMap.TryGetValue(evt.EventType, out var capTopic))
        {
            _logger.LogWarning("Unknown event type {EventType}, logging and skipping.", evt.EventType);
            await LogToDatabase(result, $"Unknown EventType: {evt.EventType}", null, "UNKNOWN", ct);
            CommitOffset(consumer, result);
            return;
        }

        // ── 3) CAP PUBLISH (Outbox Pattern) ─────────────────────
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var capPublisher = scope.ServiceProvider.GetRequiredService<ICapPublisher>();

            var integrationEvent = new PersonnelLifecycleIntegrationEvent
            {
                EventId = evt.EventId,
                EventType = evt.EventType,
                EmployeeNo = evt.EmployeeNo,
                EffectiveDate = evt.EffectiveDate,
                OccuredAtUtc = evt.OccuredAtUtc,
                CorrelationId = evt.CorrelationId,
                PublishedAtUtc = DateTimeOffset.UtcNow
            };

            using var transaction = await uow.BeginTransactionAsync(capPublisher, ct);
            await capPublisher.PublishAsync(capTopic, integrationEvent, cancellationToken: ct);
            await uow.CommitTransactionAsync(ct);

            CommitOffset(consumer, result);

            _logger.LogInformation(
                "Kafka → CAP published — EventId: {EventId}, Topic: {CapTopic}, EmployeeNo: {EmployeeNo}",
                evt.EventId, capTopic, evt.EmployeeNo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "CAP PUBLISH ERROR — EventId: {EventId}, EmployeeNo: {EmployeeNo}, " +
                "Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
                evt.EventId, evt.EmployeeNo,
                result.Topic, result.Partition.Value, result.Offset.Value);

            await LogToDatabase(result, ex.Message, ex.StackTrace, "FAILED", ct);
            CommitOffset(consumer, result);
        }
    }

    private async Task LogToDatabase(
        ConsumeResult<string, string> result,
        string errorMessage,
        string? stackTrace,
        string status,
        CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRepository<KafkaEventLog>>();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            await repo.AddAsync(new KafkaEventLog
            {
                Topic = result.Topic,
                PartitionNo = result.Partition.Value,
                Offset = result.Offset.Value,
                MessageKey = result.Message.Key,
                MessageValue = result.Message.Value,
                ErrorMessage = errorMessage,
                ErrorStackTrace = stackTrace,
                Status = status,
                CreatedAtUtc = DateTime.UtcNow
            }, ct);

            await uow.SaveChangesAsync(ct);
        }
        catch (Exception dbEx)
        {
            _logger.LogCritical(dbEx,
                "DB LOG FAILED — Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, OriginalError: {OriginalError}",
                result.Topic, result.Partition.Value, result.Offset.Value, errorMessage);
        }
    }

    private void CommitOffset(IConsumer<string, string> consumer, ConsumeResult<string, string> result)
    {
        try
        {
            consumer.StoreOffset(result);
            consumer.Commit(result);
        }
        catch (KafkaException ex)
        {
            _logger.LogError(ex, "Offset commit error — Partition: {Partition}, Offset: {Offset}",
                result.Partition.Value, result.Offset.Value);
        }
    }
}