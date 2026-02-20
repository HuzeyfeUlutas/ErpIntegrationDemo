using PersonnelAccessManagement.Domain.Events;

namespace PersonnelAccessManagement.Infrastructure.Kafka.Abstractions;

/// <summary>
/// Kafka'dan gelen event'i CAP outbox üzerinden RabbitMQ'ya iletir.
/// </summary>
public interface IKafkaCapBridge
{
    bool TryGetCapTopic(PersonnelLifecycleIntegrationEvent evt, out string capTopic);
    Task PublishAsync(PersonnelLifecycleIntegrationEvent evt, string capTopic, CancellationToken ct);
}