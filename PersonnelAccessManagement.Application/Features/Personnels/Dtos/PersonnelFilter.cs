using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Application.Features.Personnels.Dtos;

public class PersonnelFilter : FilterBase
{
    public string? Search { get; set; }
    public Campus? Campus { get; set; }
    public Title?  Title { get; set; }
}