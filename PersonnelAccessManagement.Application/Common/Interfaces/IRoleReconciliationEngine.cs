using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Application.Common.Interfaces;

/// <summary>
/// Bir kural silindiğinde/güncellendiğinde, hangi (campus, title) gruplarından
/// hangi rollerin kaldırılacağını hesaplar.
/// Personel tablosuna dokunmaz — sadece kural bazlı in-memory hesaplama yapar.
/// </summary>
public interface IRoleReconciliationEngine
{
    /// <summary>
    /// Etki alanındaki aktif kuralları yükler + DB'deki gerçek personel gruplarını çeker
    /// + in-memory plan oluşturur.
    /// </summary>
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