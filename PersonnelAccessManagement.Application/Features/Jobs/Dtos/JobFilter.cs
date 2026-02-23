using PersonnelAccessManagement.Application.Common.Models;

namespace PersonnelAccessManagement.Application.Features.Jobs.Dtos;

public class JobFilter : FilterBase
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? JobType { get; set; }
}