using PersonnelAccessManagement.Domain.Common;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Domain.Entities;

public sealed class Event : AuditableEntity<Guid>
{
    public EventType EventType { get; private set; }
    public string EmployeeNo { get; private set; } = default!;
    public DateTime OccurredAt { get; private set; }
    public string CorrelationId { get; private set; } = default!;

    public ICollection<EventLog> Logs { get; private set; } = new List<EventLog>();

    private Event() { }
    public Event(EventType eventType, string employeeNo, string correlationId)
    {
        Id = Guid.NewGuid();
        EventType = eventType;
        EmployeeNo = employeeNo.Trim();
        CorrelationId = correlationId.Trim();
    }
}

public sealed class EventLog : AuditableEntity<Guid>
{
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = default!;
    public string Status { get; private set; } = default!;
    public string? Error { get; private set; }

    private EventLog() { }
    
    public EventLog(Guid eventId, string step, string status, string? error = null)
    {
        Id = Guid.NewGuid();
        EventId = eventId;
        Status = status.Trim();
        Error = error;
    }
}