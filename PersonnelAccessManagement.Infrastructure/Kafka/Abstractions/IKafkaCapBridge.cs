using PersonnelAccessManagement.Domain.Events;

namespace PersonnelAccessManagement.Infrastructure.Kafka.Abstractions;

public interface IKafkaCapBridge
{
    bool TryGetCapTopic(PersonnelLifecycleIntegrationEvent evt, out string capTopic);
    Task PublishAsync(PersonnelLifecycleIntegrationEvent evt, string capTopic, CancellationToken ct);
}