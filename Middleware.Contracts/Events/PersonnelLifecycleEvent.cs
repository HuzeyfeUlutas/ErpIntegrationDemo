namespace Middleware.Contracts.Events;

public record PersonnelLifecycleEvent(Guid EventId, int EventType, string EmployeeNo, DateOnly EffectiveDate, DateTimeOffset OccuredAtUtc, string CorrelationId);
