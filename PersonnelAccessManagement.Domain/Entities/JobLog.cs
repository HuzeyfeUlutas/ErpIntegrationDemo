using PersonnelAccessManagement.Domain.Common;

namespace PersonnelAccessManagement.Domain.Entities;

public sealed class JobLog : AuditableEntity<Guid>
{
    public Guid JobId { get; private set; }
    public Job Job { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    public string Status { get; private set; } = default!;

    private JobLog() { }

    public JobLog(Guid jobId, string message, string status = "INFO")
    {
        Id = Guid.NewGuid();
        JobId = jobId;
        Message = message;
        Status = status;
        CreatedAt = DateTime.UtcNow;
    }
}