using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Domain.Common;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Application.Features.Rules.Dtos;

public class RuleFilter : FilterBase
{
    public string? Name { get; set; }
    public Campus? Campus { get; set; }
    public Title?  Title { get; set; }
}