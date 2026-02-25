using Confluent.Kafka;

namespace PersonnelAccessManagement.Infrastructure.Kafka.Abstractions;

public interface IKafkaEventLogger
{
    Task LogSuccessAsync(ConsumeResult<string, string> result, Guid eventId, string capTopic, CancellationToken ct);
    Task LogErrorAsync(ConsumeResult<string, string> result, string errorMessage, string? stackTrace, string status, CancellationToken ct);
}