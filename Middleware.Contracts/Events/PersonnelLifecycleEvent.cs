using Middleware.Contracts.Enums;

namespace Middleware.Contracts.Events;

public record PersonnelLifecycleEvent(
    Guid EventId, 
    PersonnelEventType EventType,  // int → enum
    string EmployeeNo, 
    DateOnly EffectiveDate, 
    DateTimeOffset OccuredAtUtc, 
    string CorrelationId);