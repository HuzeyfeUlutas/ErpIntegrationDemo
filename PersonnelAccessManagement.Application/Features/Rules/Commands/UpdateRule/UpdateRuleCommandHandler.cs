using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Exceptions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Features.Rules.Commands.UpdateRule;

public sealed class UpdateRuleCommandHandler : IRequestHandler<UpdateRuleCommand>
{
    private readonly IRepository<Rule> _rules;
    private readonly IRepository<Role> _roles;
    private readonly IUnitOfWork _uow;

    public UpdateRuleCommandHandler(
        IRepository<Rule> rules,
        IRepository<Role> roles,
        IUnitOfWork uow)
    {
        _rules = rules;
        _roles = roles;
        _uow = uow;
    }

    public async Task Handle(UpdateRuleCommand request, CancellationToken ct)
    {
        var rule = await _rules.Query()
            .Include(r => r.Roles)
            .Where(r => !r.IsDeleted)
            .FirstOrDefaultAsync(r => r.Id == request.Id, ct);

        if (rule is null)
            throw new NotFoundException($"Rule not found: {request.Id}");

        // scope unique (exclude self)
        var exists = await _rules.Query()
            .AnyAsync(r => r.Id != request.Id && r.Campus == request.Campus && r.Title == request.Title && !r.IsDeleted, ct);

        if (exists)
            throw new ConflictException("A rule with the same campus/title scope already exists.");

        var roles = await _roles.Query()
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync(ct);

        if (roles.Count != request.RoleIds.Count)
            throw new NotFoundException("One or more roles were not found.");

        // update fields
        rule.Update(request.Name, request.Campus, request.Title, request.IsActive);

        // sync roles (many-to-many)
        rule.Roles.Clear();
        
        foreach (var role in roles)
            rule.AddRole(role);

        await _uow.SaveChangesAsync(ct);
    }
}