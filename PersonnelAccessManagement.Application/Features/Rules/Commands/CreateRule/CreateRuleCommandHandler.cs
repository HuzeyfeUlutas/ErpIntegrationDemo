using AutoMapper;
using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Constants;
using PersonnelAccessManagement.Application.Common.Exceptions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Features.Events;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Features.Rules.Commands.CreateRule;

public sealed class CreateRuleCommandHandler : IRequestHandler<CreateRuleCommand, Guid>
{
    private readonly IRepository<Rule> _ruleRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IUnitOfWork _uow;
    private readonly ICapPublisher _capPublisher;
    private readonly ILogger<CreateRuleCommandHandler> _logger;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;
    
    public CreateRuleCommandHandler(
        IRepository<Rule> ruleRepository,
        IRepository<Role> roleRepository,
        IUnitOfWork uow,
        ICapPublisher capPublisher,
        ILogger<CreateRuleCommandHandler> logger,
        ICorrelationIdAccessor correlationIdAccessor)
    {
        _ruleRepository = ruleRepository;
        _roleRepository = roleRepository;
        _uow = uow;
        _capPublisher = capPublisher;
        _logger = logger;
        _correlationIdAccessor = correlationIdAccessor;
    }

    public async Task<Guid> Handle(CreateRuleCommand request, CancellationToken ct)
    {
        var correlationId = _correlationIdAccessor.CorrelationId;

        _logger.LogInformation(
            "Creating rule — Name: {Name}, Campus: {Campus}, Title: {Title}, CorrelationId: {CorrelationId}",
            request.Name, request.Campus, request.Title, correlationId);
        
        // 1) Scope unique (Campus+Title)
        var exists = await _ruleRepository.Query()
            .AnyAsync(r => r.Campus == request.Campus && r.Title == request.Title && !r.IsDeleted, ct);

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

        using var transaction = await _uow.BeginTransactionAsync(_capPublisher, ct);

        await _uow.SaveChangesAsync(ct);

        await _capPublisher.PublishAsync(CapTopics.RuleCreated, new RuleIntegrationEvent
        {
            RuleId = rule.Id,
            CorrelationId = correlationId
        }, cancellationToken: ct);
        
        await _uow.CommitTransactionAsync(ct);
        
        _logger.LogInformation(
            "Rule created — Id: {RuleId}, CorrelationId: {CorrelationId}",
            rule.Id, correlationId);

        return rule.Id;
    }
}