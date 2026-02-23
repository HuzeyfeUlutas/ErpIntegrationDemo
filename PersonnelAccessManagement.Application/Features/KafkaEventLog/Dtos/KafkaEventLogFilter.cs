using PersonnelAccessManagement.Application.Common.Models;

namespace PersonnelAccessManagement.Application.Features.KafkaEventLog.Dtos;

public class KafkaEventLogFilter : FilterBase
{
    public string? Search { get; set; }       // EmployeeNo veya Topic arama
    public string? Status { get; set; }       // FAILED, POISON
    public string? EventType { get; set; }    // Terminated, Hired, etc.
}