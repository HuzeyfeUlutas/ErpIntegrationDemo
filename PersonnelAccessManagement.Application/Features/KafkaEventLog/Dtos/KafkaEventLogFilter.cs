using PersonnelAccessManagement.Application.Common.Models;

namespace PersonnelAccessManagement.Application.Features.KafkaEventLog.Dtos;

public class KafkaEventLogFilter : FilterBase
{
    public string? Search { get; set; }      
    public string? Status { get; set; }      
    public string? EventType { get; set; }   
}