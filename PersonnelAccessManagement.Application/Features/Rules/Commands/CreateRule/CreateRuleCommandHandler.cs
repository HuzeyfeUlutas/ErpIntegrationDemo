using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Exceptions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Features.Rules.Commands.CreateRule;

public sealed class CreateRuleCommandHandler : IRequestHandler<CreateRuleCommand, Guid>
{
    private readonly IRepository<Rule> _ruleRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IUnitOfWork _uow;

    public CreateRuleCommandHandler(
        IRepository<Rule> ruleRepository,
        IRepository<Role> roleRepository,
        IUnitOfWork uow)
    {
        _ruleRepository = ruleRepository;
        _roleRepository = roleRepository;
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateRuleCommand request, CancellationToken ct)
    {
        // 1) Scope unique (Campus+Title)
        var exists = await _ruleRepository.Query()
            .AnyAsync(r => r.Campus == request.Campus && r.Title == request.Title, ct);

        if (exists)
            throw new ConflictException("A rule with the same campus/title scope already exists.");

        // 2) Role check
        var roles = await _roleRepository.Query()
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync(ct);

        if (roles.Count != request.RoleIds.Count)
            throw new NotFoundException("One or more roles were not found.");

        // 3) Create
        var rule = new Rule(request.Name, request.Campus, request.Title);

        foreach (var role in roles)
            rule.AddRole(role);

        await _ruleRepository.AddAsync(rule, ct);
        await _uow.SaveChangesAsync(ct);

        return rule.Id;
    }
}