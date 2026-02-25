namespace PersonnelAccessManagement.Application.Common.Interfaces;

public interface ICurrentUserAccessor
{
    string? EmployeeNo { get; }
    bool IsAuthenticated { get; }
}