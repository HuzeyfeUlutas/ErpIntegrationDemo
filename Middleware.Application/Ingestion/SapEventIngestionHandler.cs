using Middleware.Contracts.Enums;
using Middleware.Contracts.Events;
using Middleware.Contracts.Sap;
using MiddlewareApplication.Abstractions;

namespace MiddlewareApplication.Ingestion;

public sealed class SapEventIngestionHandler
{
    private readonly IProcessedRequestsStore _store;
    private readonly IEventPublisher _publisher;
    private readonly IPersonnelDbUpdater _dbUpdater;

    public SapEventIngestionHandler(
        IProcessedRequestsStore store,
        IEventPublisher publisher,
        IPersonnelDbUpdater dbUpdater)
    {
        _store = store;
        _publisher = publisher;
        _dbUpdater = dbUpdater;
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
        
        if (await _store.IsProcessedAsync(requestId, ct))
            return IngestionResult.Deduplicated(requestId, correlationId);
        
        if (request.EventType == PersonnelEventType.PositionChanged)
        {
            var updateResult = await _dbUpdater.UpdatePositionAsync(request.EmployeeNo, ct);

            if (!updateResult.Success)
                return IngestionResult.Rejected(
                    $"Position update failed for {request.EmployeeNo}: {updateResult.Error}");
        }
        
        var evt = new PersonnelLifecycleEvent(
            EventId: Guid.NewGuid(),
            EventType: request.EventType,
            EmployeeNo: request.EmployeeNo,
            EffectiveDate: request.EffectiveDate,
            OccuredAtUtc: DateTimeOffset.UtcNow,
            CorrelationId: correlationId
        );

        await _publisher.PublishAsync(evt, ct);
        
        await _store.MarkProcessedAsync(requestId, ct);

        return IngestionResult.Accepted(requestId, correlationId, evt.EventId);
    }
}

public sealed record IngestionResult(
    string Status,          
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