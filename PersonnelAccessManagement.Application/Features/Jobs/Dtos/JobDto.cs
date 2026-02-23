namespace PersonnelAccessManagement.Application.Features.Jobs.Dtos;

public sealed record JobDto(
    Guid Id,
    string JobType,
    string Status,
    int TotalCount,
    int SuccessCount,
    int FailureCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);