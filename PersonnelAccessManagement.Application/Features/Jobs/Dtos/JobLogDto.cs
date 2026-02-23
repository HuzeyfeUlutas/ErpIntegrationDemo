namespace PersonnelAccessManagement.Application.Features.Jobs.Dtos;

public sealed record JobLogDto(
    Guid Id,
    Guid JobId,
    string Message,
    string Status,
    DateTime CreatedAt
);