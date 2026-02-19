using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Domain.Events;

public sealed record PersonnelLifecycleIntegrationEvent
{
    public Guid EventId { get; init; }
    public PersonnelEventType EventType { get; init; }
    public string EmployeeNo { get; init; } = default!;
    public DateOnly EffectiveDate { get; init; }
    public DateTimeOffset OccuredAtUtc { get; init; }
    public string CorrelationId { get; init; } = default!;
    public DateTimeOffset PublishedAtUtc { get; init; }
}