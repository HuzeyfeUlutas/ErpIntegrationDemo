using MediatR;

namespace PersonnelAccessManagement.Application.Features.Rules.Commands.DeleteRule;

public sealed record DeleteRuleCommand(Guid Id) : IRequest;