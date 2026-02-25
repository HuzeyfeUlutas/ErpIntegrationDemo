namespace MiddlewareApplication.Abstractions;

public interface IPersonnelDbUpdater
{
    Task<PositionUpdateResult> UpdatePositionAsync(string employeeNo, CancellationToken ct);
}

public sealed record PositionUpdateResult(
    bool Success,
    string? OldCampus,
    string? OldTitle,
    string? NewCampus,
    string? NewTitle,
    string? Error);