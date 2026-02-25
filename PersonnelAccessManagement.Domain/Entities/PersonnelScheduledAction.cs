using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Domain.Entities;

public sealed class PersonnelScheduledAction
{
    public long Id { get; set; }
    // Middleware event 
    public Guid EventId { get; set; }

    public string EmployeeNo { get; set; } = default!;

    public ScheduledActionType ActionType { get; set; }
    
    public DateOnly EffectiveDate { get; set; }

    public ScheduledActionStatus Status { get; set; } = ScheduledActionStatus.Pending;

    public string CorrelationId { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DateTime? ProcessedAtUtc { get; set; }
}