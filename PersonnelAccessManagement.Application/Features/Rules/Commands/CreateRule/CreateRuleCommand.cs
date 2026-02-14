using MediatR;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Application.Features.Rules.Commands.CreateRule;

public sealed record CreateRuleCommand(
    string Name,
    Campus? Campus,
    Title? Title,
    List<decimal> RoleIds
) : IRequest<Guid>;