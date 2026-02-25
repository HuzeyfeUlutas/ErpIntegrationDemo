namespace Middleware.Infrastructure.Persistence;

public sealed class PamDbOptions
{
    public string ConnectionString { get; init; } = null!;
}