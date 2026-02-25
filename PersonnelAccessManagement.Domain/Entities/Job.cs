using PersonnelAccessManagement.Domain.Common;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Domain.Entities;

public sealed class Job : AuditableEntity<Guid>
{
    public JobType JobType { get; private set; }
    public string Status { get; private set; } = "Running";
    public int TotalCount { get; private set; }
    public int SuccessCount { get; private set; }
    public int FailureCount { get; private set; }

    public ICollection<JobLog> Logs { get; private set; } = new List<JobLog>();

    private Job() { }

    public Job(JobType jobType)
    {
        Id = Guid.NewGuid();
        JobType = jobType;
    }

    public void SetTotal(int count) => TotalCount = count;

    public void Finalize(int successCount, int failureCount)
    {
        SuccessCount = successCount;
        FailureCount = failureCount;
        Status = failureCount > 0 ? "CompletedWithErrors" : "Done";
    }

    public void MarkFailed(string reason)
    {
        Status = "Failed";
        Logs.Add(new JobLog(Id, reason, "FATAL"));
    }
}