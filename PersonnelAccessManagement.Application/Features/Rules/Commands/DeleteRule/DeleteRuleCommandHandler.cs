using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<DeleteRuleCommandHandler> _logger;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;

    public DeleteRuleCommandHandler(
        IRepository<Rule> rules,
        IUnitOfWork uow,
        ICapPublisher capPublisher,
        ILogger<DeleteRuleCommandHandler> logger,
        ICorrelationIdAccessor correlationIdAccessor)
    {
        _rules = rules;
        _uow = uow;
        _capPublisher = capPublisher;
        _logger = logger;
        _correlationIdAccessor = correlationIdAccessor;
    }

    public async Task Handle(DeleteRuleCommand request, CancellationToken ct)
    {
        var correlationId = _correlationIdAccessor.CorrelationId;
        
        var rule = await _rules.Query()
            .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, ct);

        if (rule is null)
            throw new NotFoundException($"Kural bulunamadı: {request.Id}");

        rule.SoftDelete();
        
        using var transaction = await _uow.BeginTransactionAsync(_capPublisher, ct);
        
        await _uow.SaveChangesAsync(ct);
        
        await _capPublisher.PublishAsync(
            CapTopics.RuleDeleted,
            new RuleIntegrationEvent
            {
                CorrelationId = correlationId,
                RuleId = rule.Id
            },
            cancellationToken: ct);
        
        await _uow.CommitTransactionAsync(ct);
    }
}