using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Domain.Entities;

public sealed class PersonnelScheduledAction
{
    public long Id { get; set; }

    /// <summary>
    /// Middleware'den gelen orijinal event ID'si.
    /// Idempotency kontrolü için kullanılır.
    /// </summary>
    public Guid EventId { get; set; }

    public string EmployeeNo { get; set; } = default!;

    public ScheduledActionType ActionType { get; set; }

    /// <summary>
    /// İşlemin gerçekleşeceği tarih.
    /// Hangfire job'u her gün bu tarihe göre filtreleme yapar.
    /// </summary>
    public DateOnly EffectiveDate { get; set; }

    public ScheduledActionStatus Status { get; set; } = ScheduledActionStatus.Pending;

    public string CorrelationId { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Job tarafından işlendiğinde doldurulur.
    /// </summary>
    public DateTime? ProcessedAtUtc { get; set; }
}