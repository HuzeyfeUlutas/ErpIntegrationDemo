namespace PersonnelAccessManagement.Domain.Enums;

public enum ScheduledActionType
{
    Hire = 1,
    Terminate = 2
}

public static class ScheduledActionTypeExtensions
{
    public static string ToLogMessage(this ScheduledActionType actionType)
    {
        return actionType switch
        {
            ScheduledActionType.Hire =>
                "Personel işe başlatma işlemi",

            ScheduledActionType.Terminate =>
                "Personel işten ayrılma işlemi",

            _ => "Bilinmeyen personel işlemi"
        };
    }
}