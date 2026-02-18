using System.Text.Json;
using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Constants;
using PersonnelAccessManagement.Application.Common.Exceptions;
using PersonnelAccessManagement.Application.Common.Interfaces;
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

    public UpdateRuleCommandHandler(
        IRepository<Rule> rules,
        IRepository<Role> roles,
        IRepository<Event> events,
        IUnitOfWork uow,
        ICapPublisher capPublisher)
    {
        _rules = rules;
        _roles = roles;
        _events = events;
        _uow = uow;
        _capPublisher = capPublisher;
    }

    public async Task Handle(UpdateRuleCommand request, CancellationToken ct)
    {
        var rule = await _rules.Query()
            .Include(r => r.Roles)
            .Where(r => !r.IsDeleted)
            .FirstOrDefaultAsync(r => r.Id == request.Id, ct);

        if (rule is null)
            throw new NotFoundException($"Rule not found: {request.Id}");

        // Scope unique (exclude self)
        var exists = await _rules.Query()
            .AnyAsync(r => r.Id != request.Id
                        && r.Campus == request.Campus
                        && r.Title == request.Title
                        && !r.IsDeleted, ct);

        if (exists)
            throw new ConflictException("A rule with the same campus/title scope already exists.");

        var roles = await _roles.Query()
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync(ct);

        if (roles.Count != request.RoleIds.Count)
            throw new NotFoundException("One or more roles were not found.");

        // ─── Eski durumu kaydet (reconciliation için) ───

        var correlationId = Guid.NewGuid().ToString();
        var oldRoleIds = rule.Roles.Select(r => r.Id).ToList();

        bool hasChanges = rule.Campus != request.Campus
                       || rule.Title != request.Title
                       || !oldRoleIds.OrderBy(x => x)
                              .SequenceEqual(request.RoleIds.OrderBy(x => x));

        if (hasChanges)
        {
            var oldSnapshot = JsonSerializer.Serialize(new
            {
                campus = rule.Campus,
                title = rule.Title,
                roleIds = oldRoleIds
            });

            var snapshotEvent = new Event(
                EventType.RuleUpdated,
                rule.Id.ToString(),
                correlationId,
                oldSnapshot);

            await _events.AddAsync(snapshotEvent, ct);
        }

        // ─── Güncelle ───

        rule.Update(request.Name, request.Campus, request.Title, request.IsActive);

        rule.Roles.Clear();
        foreach (var role in roles)
            rule.AddRole(role);

        await _uow.SaveChangesAsync(ct);

        // ─── Değişiklik varsa background job tetikle ───

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
    }
}