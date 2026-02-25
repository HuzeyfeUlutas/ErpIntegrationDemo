using System.ComponentModel.DataAnnotations;

namespace PersonnelAccessManagement.Application.Common.Options;

public sealed class PersonnelRoleOptions
{
    public const string SectionName = "PersonnelRoles";
    
    [Required(ErrorMessage = "PersonnelRoles:ExitingRoleId is required in appsettings.")]
    public Decimal ExitingRoleId { get; init; }
    
    [Required(ErrorMessage = "PersonnelRoles:DefaultHireRoleId is required in appsettings.")]
    public Decimal DefaultHireRoleId { get; init; }
}