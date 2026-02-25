using Hangfire.Dashboard;

namespace PersonnelAccessManagement.Infrastructure.Jobs;

public sealed class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}