namespace MiddlewareApplication.Abstractions;

public interface IProcessedRequestsStore
{
    Task<bool> IsProcessedAsync(string requestId, CancellationToken ct);
    Task MarkProcessedAsync(string requestId, CancellationToken ct);
}