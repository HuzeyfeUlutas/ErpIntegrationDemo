namespace PersonnelAccessManagement.Application.Features.Events.Dtos;

public sealed record EventLogDto(
    Guid Id,
    Guid EventId,
    decimal EmployeeNo,
    string PersonnelName,
    decimal RoleId,
    string RoleName,
    string Action,
    string Status,
    string? Error,
    DateTime CreatedAt
);