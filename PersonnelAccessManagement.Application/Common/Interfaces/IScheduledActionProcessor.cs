using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Common.Interfaces;

public interface IScheduledActionProcessor
{
    Task ProcessHireAsync(PersonnelScheduledAction action, CancellationToken ct);
    Task ProcessTerminationAsync(PersonnelScheduledAction action, CancellationToken ct);
}