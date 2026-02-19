using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Exceptions;
using PersonnelAccessManagement.Application.Common.Extensions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Features.Rules.Dtos;
using PersonnelAccessManagement.Domain.Entities;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Infrastructure.Services;

/// <summary>
/// Kural değişikliklerini personel rollerine uygulayan orkestratör.
///
/// Sorumluluklar:
///   - Event oluşturma / tamamlama
///   - Reconciliation engine'den plan alma
///   - Batch processor'a iş gönderme
///
/// Reconciliation hesaplaması → IRoleReconciliationEngine
/// Tekil personel işleme      → IPersonnelRoleBatchProcessor
/// </summary>
public sealed class PersonnelRoleService : IPersonnelRoleService
{
    private const int BatchSize = 200;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRoleReconciliationEngine _reconciliationEngine;
    private readonly IPersonnelRoleBatchProcessor _batchProcessor;
    private readonly ILogger<PersonnelRoleService> _logger;

    public PersonnelRoleService(
        IServiceScopeFactory scopeFactory,
        IRoleReconciliationEngine reconciliationEngine,
        IPersonnelRoleBatchProcessor batchProcessor,
        ILogger<PersonnelRoleService> logger)
    {
        _scopeFactory = scopeFactory;
        _reconciliationEngine = reconciliationEngine;
        _batchProcessor = batchProcessor;
        _logger = logger;
    }

    // ═══════════════════════════════════════════════════════════════
    //  CREATED
    // ═══════════════════════════════════════════════════════════════

    public async Task ApplyCreatedRuleToMatchingPersonnelAsync(
        Guid ruleId, string correlationId, CancellationToken ct = default)
    {
        var (eventId, roleIds, campus, title) =
            await SetupAsync(ruleId, EventType.RuleCreated, correlationId, ct);

        var (processed, success, fail) = await ProcessMatchingPersonnelAsync(
            campus, title,
            rolesToAdd: roleIds,
            rolesToRemove: new List<decimal>(),
            eventId, ct);

        await CompleteAsync(eventId, processed, success, fail, ct);

        _logger.LogInformation(
            "RuleCreated {RuleId} — Total:{Total} Success:{Success} Fail:{Fail}",
            ruleId, processed, success, fail);
    }

    // ═══════════════════════════════════════════════════════════════
    //  DELETED
    // ═══════════════════════════════════════════════════════════════

    public async Task ApplyDeletedRuleToMatchingPersonnelAsync(
        Guid ruleId, string correlationId, CancellationToken ct = default)
    {
        var (eventId, deletedRoleIds, campus, title) =
            await SetupAsync(ruleId, EventType.RuleDeleted, correlationId, ct);

        var plan = await _reconciliationEngine.BuildPlanAsync(
            campus, title,
            deletedRoleIds.ToHashSet(),
            excludeRuleId: ruleId,
            extraRules: null, ct);

        var (processed, success, fail) = plan.HasWork
            ? await ExecuteRemovalPlanAsync(plan, eventId, ct)
            : (0, 0, 0);

        await CompleteAsync(eventId, processed, success, fail, ct);

        _logger.LogInformation(
            "RuleDeleted {RuleId} — Total:{Total} Success:{Success} Fail:{Fail}",
            ruleId, processed, success, fail);
    }

    // ═══════════════════════════════════════════════════════════════
    //  UPDATED
    // ═══════════════════════════════════════════════════════════════

    public async Task ApplyUpdatedRuleToMatchingPersonnelAsync(
        Guid ruleId, string correlationId, CancellationToken ct = default)
    {
        var (eventId, oldSnap, newSnap) =
            await SetupForUpdateAsync(ruleId, correlationId, ct);

        bool criteriaChanged = oldSnap.Campus != newSnap.Campus
                               || oldSnap.Title != newSnap.Title;

        var rolesToAdd = criteriaChanged
            ? newSnap.RoleIds
            : newSnap.RoleIds.Except(oldSnap.RoleIds).ToList();

        var rolesToRemove = criteriaChanged
            ? oldSnap.RoleIds
            : oldSnap.RoleIds.Except(newSnap.RoleIds).ToList();

        int totalProcessed = 0, totalSuccess = 0, totalFail = 0;

        // ─── Eski kriterlere uyan personelden kaldır (criteria değiştiyse) ───
        if (criteriaChanged && rolesToRemove.Count > 0)
        {
            var plan = await _reconciliationEngine.BuildPlanAsync(
                oldSnap.Campus, oldSnap.Title,
                rolesToRemove.ToHashSet(),
                excludeRuleId: ruleId,
                extraRules: new[] { new OverlappingRuleDto(newSnap.Campus, newSnap.Title, newSnap.RoleIds) },
                ct);

            if (plan.HasWork)
            {
                var (p, s, f) = await ExecuteRemovalPlanAsync(plan, eventId, ct);
                totalProcessed += p; totalSuccess += s; totalFail += f;
            }
        }

        // ─── Yeni kriterlere uyan personele tek geçişte ekle + kaldır ───
        if (rolesToAdd.Count > 0 || (!criteriaChanged && rolesToRemove.Count > 0))
        {
            var effectiveRemove = criteriaChanged
                ? new List<decimal>()   // zaten yukarıda reconcile edildi
                : rolesToRemove;

            var (p, s, f) = await ProcessMatchingPersonnelAsync(
                newSnap.Campus, newSnap.Title,
                rolesToAdd: rolesToAdd,
                rolesToRemove: effectiveRemove,
                eventId, ct);

            totalProcessed += p; totalSuccess += s; totalFail += f;
        }

        await CompleteAsync(eventId, totalProcessed, totalSuccess, totalFail, ct);
    }

    // ═══════════════════════════════════════════════════════════════
    //  PLAN EXECUTION
    // ═══════════════════════════════════════════════════════════════

    private async Task<(int Processed, int Success, int Fail)> ExecuteRemovalPlanAsync(
        ReconciliationPlan plan, Guid eventId, CancellationToken ct)
    {
        int totalProcessed = 0, totalSuccess = 0, totalFail = 0;

        foreach (var group in plan.Groups)
        {
            foreach (var (campus, title) in group.PersonnelGroups)
            {
                var (p, s, f) = await ProcessGroupAsync(
                    campus, title,
                    rolesToAdd: new List<decimal>(),
                    rolesToRemove: group.RolesToRemove,
                    eventId, ct);

                totalProcessed += p; totalSuccess += s; totalFail += f;

                _logger.LogDebug(
                    "Reconcile ({Campus},{Title}): removed [{Roles}], processed {Count}",
                    campus, title, string.Join(",", group.RolesToRemove), p);
            }
        }

        return (totalProcessed, totalSuccess, totalFail);
    }

    // ═══════════════════════════════════════════════════════════════
    //  BATCH PROCESSING
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Wildcard destekli filtre — Created ve Update-Phase2 için.
    /// </summary>
    private async Task<(int Processed, int Success, int Fail)>
        ProcessMatchingPersonnelAsync(
            Campus? filterCampus, Title? filterTitle,
            List<decimal> rolesToAdd, List<decimal> rolesToRemove,
            Guid eventId, CancellationToken ct)
    {
        var lastEmployeeNo = 0m;
        int totalSuccess = 0, totalFail = 0, processed = 0;

        while (true)
        {
            List<Guid> personnelIds;
            decimal batchLastNo;

            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider
                    .GetRequiredService<IRepository<Personnel>>();

                var batch = await repo.QueryAsNoTracking()
                    .Where(p => !p.IsDeleted)
                    .FilterIf(p => p.Campus == filterCampus, filterCampus != null)
                    .FilterIf(p => p.Title == filterTitle, filterTitle != null)
                    .Where(p => p.EmployeeNo > lastEmployeeNo)
                    .OrderBy(p => p.EmployeeNo)
                    .Take(BatchSize)
                    .Select(p => new { p.Id, p.EmployeeNo })
                    .ToListAsync(ct);

                if (batch.Count == 0) break;

                personnelIds = batch.Select(b => b.Id).ToList();
                batchLastNo = batch[^1].EmployeeNo;
            }

            var result = await _batchProcessor.ProcessBatchAsync(
                personnelIds, rolesToAdd, rolesToRemove, eventId, ct);

            totalSuccess += result.SuccessCount;
            totalFail += result.FailCount;
            processed += personnelIds.Count;
            lastEmployeeNo = batchLastNo;
        }

        return (processed, totalSuccess, totalFail);
    }

    /// <summary>
    /// Spesifik (campus, title) grubu — Reconciliation sonucu için.
    /// </summary>
    private async Task<(int Processed, int Success, int Fail)>
        ProcessGroupAsync(
            Campus groupCampus, Title groupTitle,
            List<decimal> rolesToAdd, List<decimal> rolesToRemove,
            Guid eventId, CancellationToken ct)
    {
        var lastEmployeeNo = 0m;
        int totalSuccess = 0, totalFail = 0, processed = 0;

        while (true)
        {
            List<Guid> personnelIds;
            decimal batchLastNo;

            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider
                    .GetRequiredService<IRepository<Personnel>>();

                var batch = await repo.QueryAsNoTracking()
                    .Where(p => !p.IsDeleted)
                    .Where(p => p.Campus == groupCampus)
                    .Where(p => p.Title == groupTitle)
                    .Where(p => p.EmployeeNo > lastEmployeeNo)
                    .OrderBy(p => p.EmployeeNo)
                    .Take(BatchSize)
                    .Select(p => new { p.Id, p.EmployeeNo })
                    .ToListAsync(ct);

                if (batch.Count == 0) break;

                personnelIds = batch.Select(b => b.Id).ToList();
                batchLastNo = batch[^1].EmployeeNo;
            }

            var result = await _batchProcessor.ProcessBatchAsync(
                personnelIds, rolesToAdd, rolesToRemove, eventId, ct);

            totalSuccess += result.SuccessCount;
            totalFail += result.FailCount;
            processed += personnelIds.Count;
            lastEmployeeNo = batchLastNo;
        }

        return (processed, totalSuccess, totalFail);
    }

    // ═══════════════════════════════════════════════════════════════
    //  SETUP & COMPLETE
    // ═══════════════════════════════════════════════════════════════

    private async Task<(Guid EventId, List<decimal> RoleIds, Campus? Campus, Title? Title)>
        SetupAsync(Guid ruleId, EventType eventType, string correlationId, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;

        var ruleRepo = sp.GetRequiredService<IRepository<Rule>>();
        var eventRepo = sp.GetRequiredService<IRepository<Event>>();
        var uow = sp.GetRequiredService<IUnitOfWork>();

        var rule = await ruleRepo.QueryAsNoTracking()
                       .FirstOrDefaultAsync(r => r.Id == ruleId, ct)
                   ?? throw new NotFoundException($"Rule {ruleId} not found.");

        var roleIds = await ruleRepo.QueryAsNoTracking()
            .Where(r => r.Id == ruleId)
            .SelectMany(r => r.Roles)
            .Select(r => r.Id)
            .ToListAsync(ct);

        var sourceDetail = JsonSerializer.Serialize(new
        {
            campus = rule.Campus,
            title = rule.Title,
            roleIds
        });

        var evt = new Event(eventType, ruleId.ToString(), correlationId, sourceDetail);
        await eventRepo.AddAsync(evt, ct);
        await uow.SaveChangesAsync(ct);

        return (evt.Id, roleIds, rule.Campus, rule.Title);
    }

    private async Task<(Guid EventId, RuleSnapshot Old, RuleSnapshot New)>
        SetupForUpdateAsync(Guid ruleId, string correlationId, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;

        var ruleRepo = sp.GetRequiredService<IRepository<Rule>>();
        var eventRepo = sp.GetRequiredService<IRepository<Event>>();
        var uow = sp.GetRequiredService<IUnitOfWork>();

        var rule = await ruleRepo.QueryAsNoTracking()
                       .FirstOrDefaultAsync(r => r.Id == ruleId && !r.IsDeleted, ct)
                   ?? throw new NotFoundException($"Rule {ruleId} not found.");

        var newRoleIds = await ruleRepo.QueryAsNoTracking()
            .Where(r => r.Id == ruleId)
            .SelectMany(r => r.Roles)
            .Select(r => r.Id)
            .ToListAsync(ct);

        var newSnap = new RuleSnapshot(rule.Campus, rule.Title, newRoleIds);

        // ── Mevcut event'i bul (command handler'ın oluşturduğu) ──
        var evt = await eventRepo.Query()   // tracking açık!
                      .Where(e => e.SourceId == ruleId.ToString()
                                  && e.CorrelationId == correlationId
                                  && e.EventType == EventType.RuleUpdated)
                      .OrderByDescending(e => e.CreatedAt)
                      .FirstOrDefaultAsync(ct)
                  ?? throw new NotFoundException(
                      $"Pre-update snapshot event not found for Rule {ruleId}");

        var oldSnap = JsonSerializer.Deserialize<RuleSnapshot>(evt.SourceDetail!)
                      ?? throw new InvalidOperationException("Could not deserialize old rule snapshot.");

        // ── Aynı event'i güncelle, yenisini oluşturma ──
        var sourceDetail = JsonSerializer.Serialize(new
        {
            oldCampus = oldSnap.Campus, oldTitle = oldSnap.Title, oldRoleIds = oldSnap.RoleIds,
            newCampus = newSnap.Campus, newTitle = newSnap.Title, newRoleIds = newSnap.RoleIds
        });

        evt.UpdateSourceDetail(sourceDetail);
        await uow.SaveChangesAsync(ct);

        return (evt.Id, oldSnap, newSnap);
    }

    private async Task CompleteAsync(
        Guid eventId, int total, int success, int fail, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;

        var eventRepo = sp.GetRequiredService<IRepository<Event>>();
        var uow = sp.GetRequiredService<IUnitOfWork>();

        var evt = await eventRepo.Query().FirstAsync(e => e.Id == eventId, ct);
        evt.Complete(total, success, fail);
        await uow.SaveChangesAsync(ct);
    }
}