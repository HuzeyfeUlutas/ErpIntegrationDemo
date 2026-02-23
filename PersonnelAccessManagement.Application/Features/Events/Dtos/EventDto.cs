namespace PersonnelAccessManagement.Application.Features.Events.Dtos;

public sealed record EventDto(
    Guid Id,
    string EventType,
    string SourceId,
    string? SourceDetail,
    string CorrelationId,
    DateTime OccurredAt,
    int TotalCount,
    int SuccessCount,
    int FailCount,
    bool IsCompleted,
    DateTime CreatedAt
);