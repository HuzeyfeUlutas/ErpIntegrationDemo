using System.Text.Json;
using Confluent.Kafka;
using Middleware.Contracts.Events;
using MiddlewareApplication.Abstractions;

namespace Middleware.Infrastructure.Kafka;

public sealed class KafkaProducer : IDisposable, IEventPublisher
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaOptions _opt;

    public KafkaProducer(KafkaOptions opt)
    {
        _opt = opt;
        var config = new ProducerConfig { BootstrapServers = opt.BootstrapServers };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public async Task PublishAsync(PersonnelLifecycleEvent evt, CancellationToken ct)
    {
        var key = evt.EmployeeNo;
        var payload = JsonSerializer.Serialize(evt, JsonOpts);
        await _producer.ProduceAsync(_opt.Topic, new Message<string, string> { Key = key, Value = payload }, ct);
    }

    public void Dispose() => _producer.Dispose();
}