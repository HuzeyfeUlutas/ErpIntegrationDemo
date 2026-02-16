using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Application.Common.Interfaces;

public interface IPersonnelRoleService
{
    Task ApplyRuleToMatchingPersonnelAsync(
        Guid ruleId,
        string correlationId,
        CancellationToken ct = default);
}