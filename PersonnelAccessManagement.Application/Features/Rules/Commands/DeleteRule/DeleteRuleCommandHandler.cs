using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Constants;
using PersonnelAccessManagement.Application.Common.Exceptions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Features.Rules.Events;
using Rule = PersonnelAccessManagement.Domain.Entities.Rule;

namespace PersonnelAccessManagement.Application.Features.Rules.Commands.DeleteRule;

public sealed class DeleteRuleCommandHandler : IRequestHandler<DeleteRuleCommand>
{
    private readonly IRepository<Rule> _rules;
    private readonly IUnitOfWork _uow;
    private readonly ICapPublisher _capPublisher;

    public DeleteRuleCommandHandler(
        IRepository<Rule> rules,
        IUnitOfWork uow,
        ICapPublisher capPublisher)
    {
        _rules = rules;
        _uow = uow;
        _capPublisher = capPublisher;
    }

    public async Task Handle(DeleteRuleCommand request, CancellationToken ct)
    {
        var rule = await _rules.Query()
            .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, ct);

        if (rule is null)
            throw new NotFoundException($"Rule not found: {request.Id}");

        rule.SoftDelete();

        var correlationId = Guid.NewGuid().ToString();

        await _uow.SaveChangesAsync(ct);

        // Soft-delete sonrası publish — PersonnelRoleService kuralı hâlâ okuyabilir
        await _capPublisher.PublishAsync(
            CapTopics.RuleDeleted,
            new RuleIntegrationEvent
            {
                CorrelationId = correlationId,
                RuleId = rule.Id
            },
            cancellationToken: ct);
    }
}