using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Domain.Entities;
using PersonnelAccessManagement.Infrastructure.Kafka.Abstractions;

namespace PersonnelAccessManagement.Infrastructure.Kafka;

public sealed class KafkaEventLogger : IKafkaEventLogger
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<KafkaEventLogger> _logger;

    public KafkaEventLogger(
        IServiceScopeFactory scopeFactory,
        ILogger<KafkaEventLogger> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task LogSuccessAsync(
        ConsumeResult<string, string> result,
        Guid eventId,
        string capTopic,
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
                ErrorMessage = "",
                Status = "SUCCESS",
                CreatedAtUtc = DateTime.UtcNow
            }, ct);

            await uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "SUCCESS LOG FAILED — Partition: {Partition}, Offset: {Offset}. Non-critical, continuing.",
                result.Partition.Value, result.Offset.Value);
        }
    }

    public async Task LogErrorAsync(
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
        catch (Exception ex)
        {
            _logger.LogCritical(ex,
                "DB LOG FAILED — Partition: {Partition}, Offset: {Offset}, OriginalError: {Error}",
                result.Partition.Value, result.Offset.Value, errorMessage);
        }
    }
}