using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Constants;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Domain.Entities;
using PersonnelAccessManagement.Domain.Events;

namespace PersonnelAccessManagement.Infrastructure.EventHandlers;

public sealed class PositionChangedEventHandler : ICapSubscribe
{
    private readonly IRepository<Personnel> _personnelRepo;
    private readonly IRepository<Rule> _ruleRepo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<PositionChangedEventHandler> _logger;

    public PositionChangedEventHandler(
        IRepository<Personnel> personnelRepo,
        IRepository<Rule> ruleRepo,
        IUnitOfWork uow,
        ILogger<PositionChangedEventHandler> logger)
    {
        _personnelRepo = personnelRepo;
        _ruleRepo = ruleRepo;
        _uow = uow;
        _logger = logger;
    }

    [CapSubscribe(CapTopics.PositionChanged)]
    public async Task HandleAsync(PersonnelLifecycleIntegrationEvent @event)
    {
        _logger.LogInformation(
            "CAP received POSITION_CHANGED — EmployeeNo: {EmployeeNo}, EventId: {EventId}",
            @event.EmployeeNo, @event.EventId);

        if (!decimal.TryParse(@event.EmployeeNo, out var empNo))
        {
            _logger.LogError("Invalid EmployeeNo format: {EmployeeNo}, EventId: {EventId}",
                @event.EmployeeNo, @event.EventId);
            return;
        }

        var personnel = await _personnelRepo.Query()
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.EmployeeNo == empNo);

        if (personnel is null)
        {
            _logger.LogWarning("Personnel {EmployeeNo} not found for POSITION_CHANGED.", @event.EmployeeNo);
            return;
        }

        var oldRoleCount = personnel.Roles.Count;
        
        personnel.SetRoles(Enumerable.Empty<Role>());
        
        var matchingRules = await _ruleRepo.Query()
            .Include(r => r.Roles)
            .Where(r => r.IsActive && !r.IsDeleted)
            .Where(r => r.Campus == null || r.Campus == personnel.Campus)
            .Where(r => r.Title == null || r.Title == personnel.Title)
            .ToListAsync();

        var distinctRoles = matchingRules
            .SelectMany(r => r.Roles)
            .DistinctBy(r => r.Id)
            .ToList();
        
        foreach (var role in distinctRoles)
            personnel.AddRole(role);

        await _uow.SaveChangesAsync(CancellationToken.None);

        _logger.LogInformation(
            "POSITION_CHANGED completed — {EmployeeNo} ({Campus},{Title}): " +
            "{OldRoles} role(s) removed, {NewRoles} role(s) assigned, {RuleCount} rule(s) matched",
            @event.EmployeeNo, personnel.Campus, personnel.Title,
            oldRoleCount, distinctRoles.Count, matchingRules.Count);
    }
}