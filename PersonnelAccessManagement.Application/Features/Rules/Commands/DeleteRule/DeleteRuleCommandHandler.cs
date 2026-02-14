using System.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Exceptions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using Rule = PersonnelAccessManagement.Domain.Entities.Rule;

namespace PersonnelAccessManagement.Application.Features.Rules.Commands.DeleteRule;

public sealed class DeleteRuleCommandHandler : IRequestHandler<DeleteRuleCommand>
{
    private readonly IRepository<Rule> _rules;
    private readonly IUnitOfWork _uow;

    public DeleteRuleCommandHandler(IRepository<Rule> rules, IUnitOfWork uow)
    {
        _rules = rules;
        _uow = uow;
    }

    public async Task Handle(DeleteRuleCommand request, CancellationToken ct)
    {
        var rule = await _rules.Query()
            .FirstOrDefaultAsync(r => r.Id == request.Id, ct);

        if (rule is null)
            throw new NotFoundException($"Rule not found: {request.Id}");

        rule.SoftDelete();
        await _uow.SaveChangesAsync(ct);
    }
}