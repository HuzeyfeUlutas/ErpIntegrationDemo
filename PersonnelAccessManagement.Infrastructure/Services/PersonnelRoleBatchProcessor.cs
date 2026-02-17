// ── Batch Processor (her çağrı kendi scope'unda) ──

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Infrastructure.Services;

public sealed class PersonnelRoleBatchProcessor : IPersonnelRoleBatchProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PersonnelRoleBatchProcessor> _logger;

    public PersonnelRoleBatchProcessor(
        IServiceScopeFactory scopeFactory,
        ILogger<PersonnelRoleBatchProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<BatchResult> ProcessBatchAsync(
        List<Guid> personnelIds,
        List<decimal> roleIdsToAdd,
        List<decimal> roleIdsToRemove,
        Guid eventId,
        CancellationToken ct = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;

        var personnelRepo = sp.GetRequiredService<IRepository<Personnel>>();
        var roleRepo = sp.GetRequiredService<IRepository<Role>>();
        var eventRepo = sp.GetRequiredService<IRepository<Event>>();
        var eventLogRepo = sp.GetRequiredService<IRepository<EventLog>>();
        var uow = sp.GetRequiredService<IUnitOfWork>();

        var personnelList = await personnelRepo.Query()
            .Include(p => p.Roles)
            .Where(p => personnelIds.Contains(p.Id))
            .ToListAsync(ct);

        var allRoleIds = roleIdsToAdd.Union(roleIdsToRemove).ToList();

        var roles = await roleRepo.Query()
            .Where(r => allRoleIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, ct);

        var evt = await eventRepo.Query()
            .FirstAsync(e => e.Id == eventId, ct);

        int success = 0, fail = 0;

        foreach (var personnel in personnelList)
        {
            foreach (var roleId in roleIdsToAdd)
            {
                if (!roles.TryGetValue(roleId, out var role))
                {
                    var log = EventLog.Fail(evt.Id, personnel.EmployeeNo,
                        personnel.FullName, roleId, "Unknown", "Assigned",
                        $"Role {roleId} not found");
                    
                    evt.AddLog(log); 
                    await eventLogRepo.AddAsync(log, ct);
                    fail++;
                    continue;
                }

                try
                {
                    var log = EventLog.Success(evt.Id, personnel.EmployeeNo,
                        personnel.FullName, roleId, role.Name, "Assigned");
                    
                    personnel.AddRole(role);
                    evt.AddLog(log);
                    await eventLogRepo.AddAsync(log, ct);
                    success++;
                }
                catch (Exception ex)
                {
                    var log = EventLog.Fail(evt.Id, personnel.EmployeeNo,
                        personnel.FullName, roleId, role.Name, "Assigned", ex.Message);
                    
                    _logger.LogError(ex, "Add failed: {RoleId} → {EmployeeNo}",
                        roleId, personnel.EmployeeNo);
                    evt.AddLog(log);
                    await eventLogRepo.AddAsync(log, ct);
                    fail++;
                }
            }

            // ── Çıkar ──
            foreach (var roleId in roleIdsToRemove)
            {
                if (!roles.TryGetValue(roleId, out var role))
                {
                    var log = EventLog.Fail(evt.Id, personnel.EmployeeNo,
                        personnel.FullName, roleId, "Unknown", "Revoked",
                        $"Role {roleId} not found");
                    
                    evt.AddLog(log);
                    await eventLogRepo.AddAsync(log, ct);
                    fail++;
                    continue;
                }

                try
                {
                    var log = EventLog.Success(evt.Id, personnel.EmployeeNo,
                        personnel.FullName, roleId, role.Name, "Revoked");
                    
                    personnel.RemoveRole(role.Id);
                    evt.AddLog(log);
                    await eventLogRepo.AddAsync(log, ct);
                    success++;
                }
                catch (Exception ex)
                {
                    var log = EventLog.Fail(evt.Id, personnel.EmployeeNo,
                        personnel.FullName, roleId, role.Name, "Revoked", ex.Message);
                    
                    _logger.LogError(ex, "Remove failed: {RoleId} → {EmployeeNo}",
                        roleId, personnel.EmployeeNo);
                    evt.AddLog(log);
                    await eventLogRepo.AddAsync(log, ct);
                    fail++;
                }
            }
        }

        try
        {
            await uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                Console.WriteLine(
                    $"⚠️ Concurrency on {entry.Entity.GetType().Name} | State={entry.State} | PK={string.Join(",", entry.Properties.Where(p => p.Metadata.IsPrimaryKey()).Select(p => $"{p.Metadata.Name}={p.CurrentValue}"))}");
            }
            throw;
        }

        return new BatchResult(success, fail);
    }
}