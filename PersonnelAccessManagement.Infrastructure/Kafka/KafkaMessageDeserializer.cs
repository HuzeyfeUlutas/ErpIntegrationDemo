using System.Text.Json;
using System.Text.Json.Serialization;
using PersonnelAccessManagement.Domain.Events;
using PersonnelAccessManagement.Infrastructure.Kafka.Abstractions;

namespace PersonnelAccessManagement.Infrastructure.Kafka;

public sealed class KafkaMessageDeserializer : IKafkaMessageDeserializer<PersonnelLifecycleIntegrationEvent>
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(allowIntegerValues: true) }
    };

    public PersonnelLifecycleIntegrationEvent? Deserialize(string json)
        => JsonSerializer.Deserialize<PersonnelLifecycleIntegrationEvent>(json, Options);
}