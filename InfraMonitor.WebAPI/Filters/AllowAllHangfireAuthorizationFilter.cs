using Hangfire.Dashboard;

namespace InfraMonitor.WebAPI.Filters;

public class AllowAllHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // Allow all requests for development purposes
        System.Console.WriteLine("Hangfire Dashboard Authorization called.");
        return true;
    }
}
