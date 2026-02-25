using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Infrastructure.Jobs;

public sealed class ScheduledActionProcessor : IScheduledActionProcessor
{
    private readonly IRepository<Personnel> _personnelRepo;
    private readonly IRepository<Rule> _ruleRepo;
    private readonly ILogger<ScheduledActionProcessor> _logger;

    public ScheduledActionProcessor(
        IRepository<Personnel> personnelRepo,
        IRepository<Rule> ruleRepo,
        ILogger<ScheduledActionProcessor> logger)
    {
        _personnelRepo = personnelRepo;
        _ruleRepo = ruleRepo;
        _logger = logger;
    }
    
    public async Task ProcessHireAsync(PersonnelScheduledAction action, CancellationToken ct)
    {
        _logger.LogInformation(
            "Processing HIRE — EmployeeNo: {EmployeeNo}, EffectiveDate: {Date}",
            action.EmployeeNo, action.EffectiveDate);

        var personnel = await _personnelRepo.Query()
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.EmployeeNo == decimal.Parse(action.EmployeeNo), ct);

        if (personnel is null)
        {
            _logger.LogWarning("Personnel {EmployeeNo} not found for HIRE action.", action.EmployeeNo);
            return;
        }
        
        var matchingRules = await _ruleRepo.Query()
            .Include(r => r.Roles)
            .Where(r => r.IsActive && !r.IsDeleted)
            .Where(r => r.Campus == null || r.Campus == personnel.Campus)
            .Where(r => r.Title == null || r.Title == personnel.Title)
            .ToListAsync(ct);

        if (matchingRules.Count == 0)
        {
            _logger.LogWarning(
                "No matching rules found for {EmployeeNo} (Campus: {Campus}, Title: {Title})",
                action.EmployeeNo, personnel.Campus, personnel.Title);
            return;
        }
        
        var distinctRoles = matchingRules
            .SelectMany(r => r.Roles)
            .DistinctBy(r => r.Id)
            .ToList();
        
        var assignedCount = 0;
        foreach (var role in distinctRoles)
        {
            if (personnel.Roles.Any(r => r.Id == role.Id))
                continue;

            personnel.AddRole(role);
            assignedCount++;

            _logger.LogInformation(
                "Assigned role {RoleName} ({RoleId}) to {EmployeeNo}",
                role.Name, role.Id, action.EmployeeNo);
        }

        _personnelRepo.Update(personnel);

        _logger.LogInformation(
            "HIRE completed — {EmployeeNo}: {RuleCount} rule(s) matched, {DistinctRoles} distinct role(s), {Assigned} newly assigned",
            action.EmployeeNo, matchingRules.Count, distinctRoles.Count, assignedCount);
    }
    
    public async Task ProcessTerminationAsync(PersonnelScheduledAction action, CancellationToken ct)
    {
        _logger.LogInformation(
            "Processing TERMINATION — EmployeeNo: {EmployeeNo}, EffectiveDate: {Date}",
            action.EmployeeNo, action.EffectiveDate);

        var personnel = await _personnelRepo.Query()
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.EmployeeNo == decimal.Parse(action.EmployeeNo), ct);

        if (personnel is null)
        {
            _logger.LogWarning("Personnel {EmployeeNo} not found for TERMINATE action.", action.EmployeeNo);
            return;
        }
        
        var removedRoles = personnel.Roles.Select(r => new { r.Id, r.Name }).ToList();
        personnel.SetRoles(Enumerable.Empty<Role>());

        foreach (var role in removedRoles)
        {
            _logger.LogInformation(
                "Removed role {RoleName} ({RoleId}) from {EmployeeNo}",
                role.Name, role.Id, action.EmployeeNo);
        }
        
        personnel.SoftDelete();
        _personnelRepo.Update(personnel);

        _logger.LogInformation(
            "TERMINATION completed — {EmployeeNo}: {RoleCount} role(s) removed, personnel soft-deleted",
            action.EmployeeNo, removedRoles.Count);
    }
}