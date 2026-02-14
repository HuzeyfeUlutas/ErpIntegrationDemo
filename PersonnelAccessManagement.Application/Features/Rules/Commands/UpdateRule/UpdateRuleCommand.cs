using MediatR;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Application.Features.Rules.Commands.UpdateRule;

public sealed record UpdateRuleCommand(
    Guid Id,
    string Name,
    Campus? Campus,
    Title? Title,
    bool IsActive,
    List<decimal> RoleIds
) : IRequest;