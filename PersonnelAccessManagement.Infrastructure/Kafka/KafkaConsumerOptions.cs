using System.ComponentModel.DataAnnotations;

namespace PersonnelAccessManagement.Infrastructure.Kafka;

public sealed class KafkaConsumerOptions
{
    public const string SectionName = "Kafka";

    [Required(ErrorMessage = "Kafka:BootstrapServers is required in appsettings.")]
    public string BootstrapServers { get; init; } = null!;

    [Required(ErrorMessage = "Kafka:Topic is required in appsettings.")]
    public string Topic { get; init; } = null!;

    [Required(ErrorMessage = "Kafka:GroupId is required in appsettings.")]
    public string GroupId { get; init; } = null!;

    public string AutoOffsetReset { get; init; } = "earliest";

    public bool EnableAutoCommit { get; init; } = false;
}