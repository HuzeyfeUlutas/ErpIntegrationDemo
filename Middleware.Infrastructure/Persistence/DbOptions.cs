namespace Middleware.Infrastructure.Persistence;

public sealed class DbOptions
{
    public string ConnectionString { get; init; } = null!;
}