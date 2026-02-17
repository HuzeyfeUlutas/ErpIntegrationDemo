using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Application.Common.Interfaces;

public interface IPersonnelRoleService
{
    Task ApplyCreatedRuleToMatchingPersonnelAsync(
        Guid ruleId,
        string correlationId,
        CancellationToken ct = default);
    
    Task ApplyUpdatedRuleToMatchingPersonnelAsync(
        Guid ruleId,
        string correlationId,
        CancellationToken ct = default);
    
    Task ApplyDeletedRuleToMatchingPersonnelAsync(
        Guid ruleId,
        string correlationId,
        CancellationToken ct = default);
}