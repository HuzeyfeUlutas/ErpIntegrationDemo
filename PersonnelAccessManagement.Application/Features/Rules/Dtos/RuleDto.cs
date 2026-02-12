using PersonnelAccessManagement.Application.Features.Roles.Dtos;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Application.Features.Rules.Dtos;

public sealed record RuleDto(
    Guid Id,
    string Name,
    Campus? Campus,
    Title? Title,
    bool IsActive,
    IReadOnlyList<RoleDto> Roles
);