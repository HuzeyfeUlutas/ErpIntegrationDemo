namespace PersonnelAccessManagement.Application.Common.Options;

public sealed class AdminSettings
{
    public const string SectionName = "AdminSettings";

    public string[] AdminEmployeeNos { get; init; } = [];
}