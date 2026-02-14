using PersonnelAccessManagement.Application.Features.Roles.Dtos;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Application.Features.Personnels.Dtos;

public sealed record PersonnelDto
(
    decimal EmployeeNo,
    string FullName,
    Campus Campus,
    Title Title,
    IReadOnlyList<RoleDto> Roles
);