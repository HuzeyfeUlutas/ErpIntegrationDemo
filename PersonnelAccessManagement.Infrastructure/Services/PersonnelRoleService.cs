using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Exceptions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Domain.Entities;
using PersonnelAccessManagement.Domain.Enums;

public sealed class PersonnelRoleService : IPersonnelRoleService
{
    private const int BatchSize = 200;

    private readonly IRepository<Personnel> _personnelRepo;
    private readonly IRepository<Rule> _ruleRepo;
    private readonly IRepository<Event> _eventRepo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<PersonnelRoleService> _logger;

    public PersonnelRoleService(
        IRepository<Personnel> personnelRepo,
        IRepository<Event> eventRepo,
        IUnitOfWork uow,
        ILogger<PersonnelRoleService> logger)
    {
        _personnelRepo = personnelRepo;
        _eventRepo = eventRepo;
        _uow = uow;
        _logger = logger;
    }

    public async Task ApplyRuleToMatchingPersonnelAsync(
        Guid ruleId,
        string correlationId,
        CancellationToken ct = default)
    {
        // 1) Event kaydı
        var rule = await _ruleRepo.Query()
                       .Include(r => r.Roles)
                       .FirstOrDefaultAsync(r => r.Id == ruleId && !r.IsDeleted, ct)
                   ?? throw new NotFoundException($"Rule {ruleId} not found.");

        var campus = rule.Campus;
        var title = rule.Title;
        var roleIds = rule.Roles.Select(r => r.Id).ToList();
        var roles = rule.Roles.ToDictionary(r => r.Id);

        // Event kaydı
        var sourceDetail = JsonSerializer.Serialize(new { campus, title, roleIds });
        var evt = new Event(EventType.RuleCreated, ruleId.ToString(), correlationId, sourceDetail);
        await _eventRepo.AddAsync(evt, ct);
        await _uow.SaveChangesAsync(ct);
        
        var totalPersonnel = await _personnelRepo.Query()
            .CountAsync(p => p.Campus == campus && p.Title == title && !p.IsDeleted, ct);

        _logger.LogInformation(
            "Rule {RuleId}: {Count} matching personnel (Campus={Campus}, Title={Title})",
            ruleId, totalPersonnel, campus, title);

        int successCount = 0, failCount = 0;
        int processed = 0;

        // 4) Batch processing
        while (processed < totalPersonnel)
        {
            var batch = await _personnelRepo.Query()
                .Where(p => p.Campus == campus && p.Title == title && !p.IsDeleted)
                .Include(p => p.Roles)
                .OrderBy(p => p.EmployeeNo)
                .Skip(processed)
                .Take(BatchSize)
                .ToListAsync(ct);

            if (batch.Count == 0) break;

            foreach (var personnel in batch)
            {
                foreach (var roleId in roleIds)
                {
                    if (!roles.TryGetValue(roleId, out var role))
                    {
                        evt.AddLog(EventLog.Fail(
                            evt.Id, personnel.EmployeeNo, personnel.FullName,
                            roleId, "Unknown", "Assigned",
                            $"Role {roleId} not found in database"));
                        failCount++;
                        continue;
                    }

                    try
                    {
                        personnel.AddRole(role);

                        evt.AddLog(EventLog.Success(
                            evt.Id, personnel.EmployeeNo, personnel.FullName,
                            roleId, role.Name, "Assigned"));
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Failed: role {RoleId} → personnel {EmployeeNo}",
                            roleId, personnel.EmployeeNo);

                        evt.AddLog(EventLog.Fail(
                            evt.Id, personnel.EmployeeNo, personnel.FullName,
                            roleId, role.Name, "Assigned", ex.Message));
                        failCount++;
                    }
                }
            }

            // 5) Her batch sonrası kaydet
            await _uow.SaveChangesAsync(ct);
            processed += batch.Count;

            _logger.LogInformation("Rule {RuleId}: {Processed}/{Total}", ruleId, processed, totalPersonnel);
        }

        // 6) Event tamamla
        evt.Complete(totalPersonnel, successCount, failCount);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Rule {RuleId} done — Total: {Total}, Success: {Success}, Fail: {Fail}",
            ruleId, totalPersonnel, successCount, failCount);
    }
}