using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonnelAccessManagement.Application.Common.Extensions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Domain.Entities;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Infrastructure.Services;

/// <summary>
/// Kural bazlı rol uzlaştırma motoru.
///
/// İş akışı:
///   1) LoadOverlappingRules — Etki alanındaki aktif kuralları yükle     (1 DB sorgusu — Rule)
///   2) GetExistingGroups   — DB'deki gerçek personel gruplarını çek     (1 DB sorgusu — Personnel DISTINCT)
///   3) BuildRemovalPlan    — In-memory hesaplama                        (0 DB sorgusu)
///
/// Personel verisini değiştirmez, sadece "neyin nereye uygulanacağını" hesaplar.
/// </summary>
public sealed class RoleReconciliationEngine : IRoleReconciliationEngine
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RoleReconciliationEngine(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<ReconciliationPlan> BuildPlanAsync(
        Campus? affectedCampus,
        Title? affectedTitle,
        HashSet<decimal> candidateRoleIds,
        Guid? excludeRuleId,
        IEnumerable<OverlappingRuleDto>? extraRules,
        CancellationToken ct = default)
    {
        // 1) Kesişen aktif kuralları yükle
        var overlappingRules = await LoadOverlappingRulesAsync(
            affectedCampus, affectedTitle, excludeRuleId, ct);

        if (extraRules is not null)
        {
            foreach (var extra in extraRules)
                overlappingRules.Add(extra);
        }

        // 2) DB'deki gerçek personel gruplarını çek
        var existingGroups = await GetExistingPersonnelGroupsAsync(
            affectedCampus, affectedTitle, ct);

        if (existingGroups.Count == 0)
            return new ReconciliationPlan(new List<ReconciliationGroup>());

        // 3) In-memory plan oluştur
        var groups = CalculateRemovalPlan(
            affectedCampus, affectedTitle,
            candidateRoleIds, overlappingRules, existingGroups);

        return new ReconciliationPlan(groups);
    }

    // ─────────────────────────────────────────────────────────
    //  DB Queries
    // ─────────────────────────────────────────────────────────

    private async Task<List<OverlappingRuleDto>> LoadOverlappingRulesAsync(
        Campus? affectedCampus, Title? affectedTitle,
        Guid? excludeRuleId, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var ruleRepo = scope.ServiceProvider.GetRequiredService<IRepository<Rule>>();

        var query = ruleRepo.QueryAsNoTracking()
            .Where(r => !r.IsDeleted);

        if (excludeRuleId.HasValue)
            query = query.Where(r => r.Id != excludeRuleId.Value);

        if (affectedCampus.HasValue)
            query = query.Where(r => r.Campus == null || r.Campus == affectedCampus.Value);

        if (affectedTitle.HasValue)
            query = query.Where(r => r.Title == null || r.Title == affectedTitle.Value);

        return await query
            .Select(r => new OverlappingRuleDto(
                r.Campus,
                r.Title,
                r.Roles.Select(role => role.Id).ToList()))
            .ToListAsync(ct);
    }

    private async Task<HashSet<(Campus, Title)>> GetExistingPersonnelGroupsAsync(
        Campus? filterCampus, Title? filterTitle, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IRepository<Personnel>>();

        var groups = await repo.QueryAsNoTracking()
            .Where(p => !p.IsDeleted)
            .FilterIf(p => p.Campus == filterCampus, filterCampus != null)
            .FilterIf(p => p.Title == filterTitle, filterTitle != null)
            .Select(p => new { p.Campus, p.Title })
            .Distinct()
            .ToListAsync(ct);

        return groups.Select(g => (g.Campus, g.Title)).ToHashSet();
    }

    // ─────────────────────────────────────────────────────────
    //  Pure Calculation (no DB, static)
    // ─────────────────────────────────────────────────────────

    private static List<ReconciliationGroup> CalculateRemovalPlan(
        Campus? affectedCampus, Title? affectedTitle,
        HashSet<decimal> candidateRoleIds,
        List<OverlappingRuleDto> overlappingRules,
        HashSet<(Campus, Title)> existingGroups)
    {
        var campusValues = affectedCampus.HasValue
            ? new[] { affectedCampus.Value }
            : Enum.GetValues<Campus>();

        var titleValues = affectedTitle.HasValue
            ? new[] { affectedTitle.Value }
            : Enum.GetValues<Title>();

        var planMap = new Dictionary<string, ReconciliationGroup>();

        foreach (var c in campusValues)
        foreach (var t in titleValues)
        {
            if (!existingGroups.Contains((c, t)))
                continue;

            var effectiveRoleIds = overlappingRules
                .Where(r => (r.Campus == null || r.Campus == c)
                         && (r.Title == null || r.Title == t))
                .SelectMany(r => r.RoleIds)
                .ToHashSet();

            var rolesToRemove = candidateRoleIds
                .Where(id => !effectiveRoleIds.Contains(id))
                .OrderBy(id => id)
                .ToList();

            if (rolesToRemove.Count == 0)
                continue;

            var key = string.Join(",", rolesToRemove);

            if (!planMap.TryGetValue(key, out var group))
            {
                group = new ReconciliationGroup(rolesToRemove, new List<(Campus, Title)>());
                planMap[key] = group;
            }

            group.PersonnelGroups.Add((c, t));
        }

        return planMap.Values.ToList();
    }
}