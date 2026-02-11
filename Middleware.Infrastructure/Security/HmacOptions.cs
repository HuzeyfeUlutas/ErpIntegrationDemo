namespace Middleware.Infrastructure.Security;

public class HmacOptions
{
    public string WebhookSecret { get; init; } = null!;
    public int AllowedClockSkewSeconds { get; init; } = 300;
}