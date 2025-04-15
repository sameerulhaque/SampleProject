using Hangfire.Dashboard;

namespace SampleProject.API.InjectionUsages;

public class DashboardNoAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}