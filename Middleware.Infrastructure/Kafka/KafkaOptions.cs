namespace Middleware.Infrastructure.Kafka;

public sealed class KafkaOptions
{
    public string BootstrapServers { get; init; } = null!;
    public string Topic { get; init; } = "erp.personnel.lifecycle";
}