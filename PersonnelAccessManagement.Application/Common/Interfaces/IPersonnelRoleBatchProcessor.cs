using PersonnelAccessManagement.Application.Common.Models;

namespace PersonnelAccessManagement.Application.Common.Interfaces;

public interface IPersonnelRoleBatchProcessor
{
    Task<BatchResult> ProcessBatchAsync(
        List<Guid> personnelIds,
        List<decimal> roleIdsToAdd,
        List<decimal> roleIdsToRemove,
        Guid eventId,
        CancellationToken ct = default);
}