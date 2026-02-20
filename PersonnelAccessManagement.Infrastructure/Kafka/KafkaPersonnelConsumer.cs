using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersonnelAccessManagement.Domain.Events;
using PersonnelAccessManagement.Infrastructure.Kafka.Abstractions;

namespace PersonnelAccessManagement.Infrastructure.Kafka;

public sealed class KafkaPersonnelConsumer : BackgroundService
{
    private readonly IKafkaMessageDeserializer<PersonnelLifecycleIntegrationEvent> _deserializer;
    private readonly IKafkaCapBridge _bridge;
    private readonly IKafkaEventLogger _eventLogger;
    private readonly KafkaConsumerOptions _options;
    private readonly ILogger<KafkaPersonnelConsumer> _logger;

    public KafkaPersonnelConsumer(
        IKafkaMessageDeserializer<PersonnelLifecycleIntegrationEvent> deserializer,
        IKafkaCapBridge bridge,
        IKafkaEventLogger eventLogger,
        IOptions<KafkaConsumerOptions> options,
        ILogger<KafkaPersonnelConsumer> logger)
    {
        _deserializer = deserializer;
        _bridge = bridge;
        _eventLogger = eventLogger;
        _options = options.Value;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Kafka consumer starting — Topic: {Topic}, GroupId: {GroupId}, Servers: {Servers}",
            _options.Topic, _options.GroupId, _options.BootstrapServers);

        return Task.Factory.StartNew(
            () => ConsumeLoopAsync(stoppingToken),
            stoppingToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default).Unwrap();
    }

    private async Task ConsumeLoopAsync(CancellationToken ct)
    {
        using var consumer = BuildConsumer();
        consumer.Subscribe(_options.Topic);

        try
        {
            while (!ct.IsCancellationRequested)
            {
                var result = TryConsume(consumer, ct);
                if (result is null) continue;

                await ProcessMessageAsync(result, consumer, ct);
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

    private async Task ProcessMessageAsync(
        ConsumeResult<string, string> result,
        IConsumer<string, string> consumer,
        CancellationToken ct)
    {
        // 1) Deserialize
        var (evt, deserializeError) = TryDeserialize(result);

        if (deserializeError is not null)
        {
            await _eventLogger.LogErrorAsync(result, deserializeError.Message, deserializeError.StackTrace, "POISON", ct);
            CommitOffset(consumer, result);
            return;
        }

        if (evt is null)
        {
            await _eventLogger.LogErrorAsync(result, "Deserialized to null", null, "POISON", ct);
            CommitOffset(consumer, result);
            return;
        }

        // 2) Topic mapping
        if (!_bridge.TryGetCapTopic(evt, out var capTopic))
        {
            _logger.LogWarning("Unknown event type {EventType}, skipping.", evt.EventType);
            await _eventLogger.LogErrorAsync(result, $"Unknown EventType: {evt.EventType}", null, "UNKNOWN", ct);
            CommitOffset(consumer, result);
            return;
        }

        // 3) CAP publish (outbox)
        try
        {
            await _bridge.PublishAsync(evt, capTopic, ct);
            await _eventLogger.LogSuccessAsync(result, evt.EventId, capTopic, ct);
            CommitOffset(consumer, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "CAP PUBLISH ERROR — EventId: {EventId}, EmployeeNo: {EmployeeNo}, Offset: {Offset}",
                evt.EventId, evt.EmployeeNo, result.Offset.Value);

            await _eventLogger.LogErrorAsync(result, ex.Message, ex.StackTrace, "FAILED", ct);
            CommitOffset(consumer, result);
        }
    }

    #region Private Helpers

    private IConsumer<string, string> BuildConsumer()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            GroupId = _options.GroupId,
            AutoOffsetReset = Enum.Parse<AutoOffsetReset>(_options.AutoOffsetReset, ignoreCase: true),
            EnableAutoCommit = _options.EnableAutoCommit,
            EnableAutoOffsetStore = false
        };

        return new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) =>
                _logger.LogError("Kafka error: {Reason} (IsFatal: {IsFatal})", e.Reason, e.IsFatal))
            .SetPartitionsAssignedHandler((_, partitions) =>
                _logger.LogInformation("Partitions assigned: {Partitions}", string.Join(", ", partitions)))
            .SetPartitionsRevokedHandler((_, partitions) =>
                _logger.LogInformation("Partitions revoked: {Partitions}", string.Join(", ", partitions)))
            .Build();
    }

    private ConsumeResult<string, string>? TryConsume(IConsumer<string, string> consumer, CancellationToken ct)
    {
        try
        {
            var result = consumer.Consume(ct);
            return result?.Message?.Value is not null ? result : null;
        }
        catch (ConsumeException ex)
        {
            _logger.LogError(ex, "Kafka consume error on topic {Topic}", _options.Topic);
            return null;
        }
    }

    private (PersonnelLifecycleIntegrationEvent? evt, Exception? error) TryDeserialize(
        ConsumeResult<string, string> result)
    {
        try
        {
            var evt = _deserializer.Deserialize(result.Message.Value);
            return (evt, null);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex,
                "DESERIALIZE ERROR — Partition: {Partition}, Offset: {Offset}, Value: {Value}",
                result.Partition.Value, result.Offset.Value, result.Message.Value);
            return (null, ex);
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

    #endregion
}