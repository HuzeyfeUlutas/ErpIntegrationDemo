namespace PersonnelAccessManagement.Application.Common.Constants;

public static class CapTopics
{
    public const string RuleCreated = "pam.rule.created";
    public const string RuleDeleted = "pam.rule.deleted";
    public const string RuleUpdated = "pam.rule.updated";
    public const string Hired = "personnel.lifecycle.hired";
    public const string Terminated = "personnel.lifecycle.terminated";
    public const string PositionChanged = "personnel.lifecycle.positionchanged";
}