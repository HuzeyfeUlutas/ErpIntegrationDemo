namespace PersonnelAccessManagement.Application.Common.Interfaces;

public interface IScheduledActionJob
{
    Task ExecuteAsync(CancellationToken ct);
}