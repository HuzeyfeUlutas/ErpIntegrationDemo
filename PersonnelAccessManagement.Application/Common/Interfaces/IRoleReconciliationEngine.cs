using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Application.Common.Interfaces;

public interface IRoleReconciliationEngine
{
    Task<ReconciliationPlan> BuildPlanAsync(
        Campus? affectedCampus,
        Title? affectedTitle,
        HashSet<decimal> candidateRoleIds,
        Guid? excludeRuleId,
        IEnumerable<OverlappingRuleDto>? extraRules,
        CancellationToken ct = default);
}

public sealed record OverlappingRuleDto(Campus? Campus, Title? Title, List<decimal> RoleIds);

public sealed record ReconciliationPlan(
    List<ReconciliationGroup> Groups)
{
    public bool HasWork => Groups.Count > 0;
}

public sealed record ReconciliationGroup(
    List<decimal> RolesToRemove,
    List<(Campus Campus, Title Title)> PersonnelGroups);