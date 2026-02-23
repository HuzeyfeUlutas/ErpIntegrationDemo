using PersonnelAccessManagement.Application.Common.Models;

namespace PersonnelAccessManagement.Application.Features.Events.Dtos;

public class EventFilter : FilterBase
{
    public string? Search { get; set; }
    public string? EventType { get; set; }
    public bool? IsCompleted { get; set; }
}