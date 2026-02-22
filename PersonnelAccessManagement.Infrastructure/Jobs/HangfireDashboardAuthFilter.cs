using Hangfire.Dashboard;

namespace PersonnelAccessManagement.Infrastructure.Jobs;

public sealed class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // TODO: Production'da JWT/role check ekle
        // var httpContext = context.GetHttpContext();
        // return httpContext.User.IsInRole("Admin");

        return true;
    }
}