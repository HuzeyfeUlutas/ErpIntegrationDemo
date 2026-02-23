namespace PersonnelAccessManagement.Domain.Entities;

public sealed class KafkaEventLog
{
    public long Id { get; set; }
    public string Topic { get; set; } = default!;
    public int PartitionNo { get; set; }
    public long Offset { get; set; }
    public string? MessageKey { get; set; }
    public string? MessageValue { get; set; }
    public string ErrorMessage { get; set; } = default!;
    public string? ErrorStackTrace { get; set; }
    public string Status { get; set; } = "FAILED"; // FAILED , SUCCESS
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public int RetryCount { get; set; }
}