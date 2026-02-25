namespace PersonnelAccessManagement.Infrastructure.Common.Options;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string ExchangeName { get; set; } = "cap.default.exchange";
}