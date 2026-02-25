using System.Text.Json;
using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Constants;
using PersonnelAccessManagement.Application.Common.Exceptions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Features.Rules.Dtos;
using PersonnelAccessManagement.Application.Features.Rules.Events;
using PersonnelAccessManagement.Domain.Entities;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Application.Features.Rules.Commands.UpdateRule;

public sealed class UpdateRuleCommandHandler : IRequestHandler<UpdateRuleCommand>
{
    private readonly IRepository<Rule> _rules;
    private readonly IRepository<Role> _roles;
    private readonly IRepository<Event> _events;
    private readonly IUnitOfWork _uow;
    private readonly ICapPublisher _capPublisher;
    private readonly ILogger<UpdateRuleCommandHandler> _logger;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;

    public UpdateRuleCommandHandler(
        IRepository<Rule> rules,
        IRepository<Role> roles,
        IRepository<Event> events,
        IUnitOfWork uow,
        ICapPublisher capPublisher,
        ILogger<UpdateRuleCommandHandler> logger,
        ICorrelationIdAccessor correlationIdAccessor)
    {
        _rules = rules;
        _roles = roles;
        _events = events;
        _uow = uow;
        _capPublisher = capPublisher;
        _logger = logger;
        _correlationIdAccessor = correlationIdAccessor;
    }

    public async Task Handle(UpdateRuleCommand request, CancellationToken ct)
    {
        var correlationId = _correlationIdAccessor.CorrelationId;
        
        var rule = await _rules.Query()
            .Include(r => r.Roles)
            .Where(r => !r.IsDeleted)
            .FirstOrDefaultAsync(r => r.Id == request.Id, ct);

        if (rule is null)
            throw new NotFoundException($"Kural bulunamadı: {request.Id}");
        
        var exists = await _rules.Query()
            .AnyAsync(r => r.Id != request.Id
                        && r.Campus == request.Campus
                        && r.Title == request.Title
                        && !r.IsDeleted, ct);

        if (exists)
            throw new ConflictException("Aynı kampüs ve unvan kapsamında zaten bir kural mevcut.");


        var roles = await _roles.Query()
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync(ct);

        if (roles.Count != request.RoleIds.Count)
            throw new NotFoundException("Bir veya daha fazla rol bulunamadı.");
        
        var oldRoleIds = rule.Roles.Select(r => r.Id).ToList();

        bool hasChanges = rule.Campus != request.Campus
                       || rule.Title != request.Title
                       || !oldRoleIds.OrderBy(x => x)
                              .SequenceEqual(request.RoleIds.OrderBy(x => x));
        
        
        using var transaction = await _uow.BeginTransactionAsync(_capPublisher, ct);


        if (hasChanges)
        {
            var oldSnapshot = JsonSerializer.Serialize(new RuleSnapshot(rule.Campus, rule.Title, oldRoleIds));

            var snapshotEvent = new Event(
                EventType.RuleUpdated,
                rule.Id.ToString(),
                correlationId,
                oldSnapshot);

            await _events.AddAsync(snapshotEvent, ct);
        }
        
        rule.Update(request.Name, request.Campus, request.Title, request.IsActive);

        rule.Roles.Clear();
        foreach (var role in roles)
            rule.AddRole(role);

        await _uow.SaveChangesAsync(ct);
        
        if (hasChanges)
        {
            await _capPublisher.PublishAsync(
                CapTopics.RuleUpdated,
                new RuleIntegrationEvent
                {
                    RuleId = rule.Id,
                    CorrelationId = correlationId
                },
                cancellationToken: ct);
        }
        
        await _uow.CommitTransactionAsync(ct);
    }
}