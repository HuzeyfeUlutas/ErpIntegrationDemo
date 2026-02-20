namespace PersonnelAccessManagement.Infrastructure.Kafka.Abstractions;

public interface IKafkaMessageDeserializer<T> where T : class
{
    T? Deserialize(string json);
}