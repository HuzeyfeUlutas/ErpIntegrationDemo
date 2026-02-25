using PersonnelAccessManagement.Domain.Common;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Domain.Entities;

public sealed class EventLog : AuditableEntity<Guid>
{
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = default!;

    public decimal EmployeeNo { get; private set; }
    public string PersonnelName { get; private set; } = default!;
    public Decimal RoleId { get; private set; }
    public string RoleName { get; private set; } = default!;
    public string Action { get; private set; } = default!;
    public EventLogStatus Status { get; private set; }
    public string? Error { get; private set; }

    private EventLog() { }

    public static EventLog Success(
        Guid eventId, decimal employeeNo, string personnelName,
        Decimal roleId, string roleName, string action)
    {
        return new EventLog
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            EmployeeNo = employeeNo,
            PersonnelName = personnelName,
            RoleId = roleId,
            RoleName = roleName,
            Action = action,
            Status = EventLogStatus.Success
        };
    }

    public static EventLog Fail(
        Guid eventId, decimal employeeNo, string personnelName,
        Decimal roleId, string roleName, string action, string error)
    {
        return new EventLog
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            EmployeeNo = employeeNo,
            PersonnelName = personnelName,
            RoleId = roleId,
            RoleName = roleName,
            Action = action,
            Status = EventLogStatus.Failed,
            Error = error
        };
    }
}