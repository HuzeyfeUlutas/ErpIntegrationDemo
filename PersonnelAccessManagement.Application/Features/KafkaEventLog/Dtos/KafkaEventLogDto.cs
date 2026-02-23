namespace PersonnelAccessManagement.Application.Features.KafkaEventLog.Dtos;

public sealed record KafkaEventLogDto(
    long Id,
    string Topic,
    int PartitionNo,
    long Offset,
    string? MessageKey,
    string Status,
    int RetryCount,
    string ErrorMessage,
    string? ErrorStackTrace,
    DateTime CreatedAtUtc,
    // MessageValue'dan parse edilen alanlar
    string? EventType,
    string? EmployeeNo,
    string? EffectiveDate,
    string? OccuredAtUtc,
    string? CorrelationId,
    string? EventId
);