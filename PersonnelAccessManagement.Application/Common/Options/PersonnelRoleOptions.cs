using System.ComponentModel.DataAnnotations;

public sealed class PersonnelRoleOptions
{
    public const string SectionName = "PersonnelRoles";

    /// <summary>
    /// İşten ayrılacak personele atanacak rolün ID'si.
    /// </summary>
    [Required(ErrorMessage = "PersonnelRoles:ExitingRoleId is required in appsettings.")]
    public Decimal ExitingRoleId { get; init; }

    /// <summary>
    /// İşe yeni başlayan personele atanacak varsayılan rolün ID'si.
    /// </summary>
    [Required(ErrorMessage = "PersonnelRoles:DefaultHireRoleId is required in appsettings.")]
    public Decimal DefaultHireRoleId { get; init; }
}