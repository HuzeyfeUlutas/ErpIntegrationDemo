using PersonnelAccessManagement.Domain.Common;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Domain.Entities;

public sealed class Job : AuditableEntity<Guid>
{
    public JobType JobType { get; private set; }
    public string Status { get; private set; } = "Running"; //Running/Done/Failed
    public int TotalCount { get; private set; }
    public int SuccessCount { get; private set; }
    public int FailureCount { get; private set; }

    public ICollection<JobLog> Logs { get; private set; } = new List<JobLog>();

    private Job() { }
    public Job(JobType jobType)
    {
        Id = Guid.NewGuid();
        JobType = jobType;
        CreatedAt = DateTime.UtcNow;
    }
}

public sealed class JobLog : AuditableEntity<Guid>
{
    public Guid JobId { get; private set; }
    public Job Job { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    public string Status { get; private set; } = default!;

    private JobLog() { }
    public JobLog(Guid jobId, string message)
    {
        Id = Guid.NewGuid();
        JobId = jobId;
        Message = message;
    }
}