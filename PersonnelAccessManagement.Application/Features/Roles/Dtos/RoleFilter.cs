using PersonnelAccessManagement.Application.Common.Models;

namespace PersonnelAccessManagement.Application.Features.Roles.Dtos;

public class RoleFilter : FilterBase
{
    public string? Name { get; set; }
}