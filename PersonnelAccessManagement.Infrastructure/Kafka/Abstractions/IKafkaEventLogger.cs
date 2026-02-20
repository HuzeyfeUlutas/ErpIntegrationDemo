using Confluent.Kafka;

namespace PersonnelAccessManagement.Infrastructure.Kafka.Abstractions;

/// <summary>
/// Kafka mesajlarını veritabanına loglar (başarılı + hatalı).
/// </summary>
public interface IKafkaEventLogger
{
    Task LogSuccessAsync(ConsumeResult<string, string> result, Guid eventId, string capTopic, CancellationToken ct);
    Task LogErrorAsync(ConsumeResult<string, string> result, string errorMessage, string? stackTrace, string status, CancellationToken ct);
}