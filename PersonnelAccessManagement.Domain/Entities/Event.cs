// Domain/Entities/Event.cs

using PersonnelAccessManagement.Domain.Common;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Domain.Entities;

public sealed class Event : AuditableEntity<Guid>
{
    public EventType EventType { get; private set; }
    public string SourceId { get; private set; } = default!;
    public string? SourceDetail { get; private set; }
    public string CorrelationId { get; private set; } = default!;
    public DateTime OccurredAt { get; private set; }

    public int TotalCount { get; private set; }
    public int SuccessCount { get; private set; }
    public int FailCount { get; private set; }
    public bool IsCompleted { get; private set; }

    public ICollection<EventLog> Logs { get; private set; } = new List<EventLog>();

    private Event() { }

    public Event(EventType eventType, string sourceId, string correlationId, string? sourceDetail = null)
    {
        Id = Guid.NewGuid();
        EventType = eventType;
        SourceId = sourceId.Trim();
        SourceDetail = sourceDetail;
        CorrelationId = correlationId.Trim();
        OccurredAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void UpdateSourceDetail(string sourceDetail)
    {
        SourceDetail = sourceDetail;
    }

    public void AddLog(EventLog log) => Logs.Add(log);

    public void Complete(int total, int success, int fail)
    {
        TotalCount = total;
        SuccessCount = success;
        FailCount = fail;
        IsCompleted = true;
    }
}