using Middleware.Contracts.Events;
using Middleware.Contracts.Sap;
using MiddlewareApplication.Abstractions;

namespace MiddlewareApplication.Ingestion;

public sealed class SapEventIngestionHandler
{
    private readonly IProcessedRequestsStore _store;
    private readonly IEventPublisher _publisher;

    public SapEventIngestionHandler(IProcessedRequestsStore store, IEventPublisher publisher)
    {
        _store = store;
        _publisher = publisher;
    }

    public async Task<IngestionResult> HandleAsync(
        SapHrEventRequest request,
        string requestId,
        string correlationId,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(requestId))
            return IngestionResult.Rejected("X-Request-Id is required.");

        if (string.IsNullOrWhiteSpace(correlationId))
            correlationId = requestId;

        // Idempotency check
        if (await _store.IsProcessedAsync(requestId, ct))
            return IngestionResult.Deduplicated(requestId, correlationId);

        // Minimal mapping to canonical event
        var evt = new PersonnelLifecycleEvent(
            EventId: Guid.NewGuid(),
            EventType: request.EventType,
            EmployeeNo: request.EmployeeNo,
            EffectiveDate: request.EffectiveDate,
            OccuredAtUtc: DateTimeOffset.UtcNow,
            CorrelationId: correlationId
        );

        await _publisher.PublishAsync(evt, ct);

        // Mark processed AFTER publish succeeds
        await _store.MarkProcessedAsync(requestId, ct);

        return IngestionResult.Accepted(requestId, correlationId, evt.EventId);
    }
}

public sealed record IngestionResult(
    string Status,            // "accepted" | "deduplicated" | "rejected"
    string RequestId,
    string CorrelationId,
    Guid? EventId,
    string? Reason)
{
    public static IngestionResult Accepted(string requestId, string correlationId, Guid eventId)
        => new("accepted", requestId, correlationId, eventId, null);

    public static IngestionResult Deduplicated(string requestId, string correlationId)
        => new("deduplicated", requestId, correlationId, null, null);

    public static IngestionResult Rejected(string reason)
        => new("rejected", "", "", null, reason);
}