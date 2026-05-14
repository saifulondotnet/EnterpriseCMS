using Hangfire.Dashboard;

namespace EnterpriseCMS.Web.Extensions;

public class HangfireAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return httpContext.User.IsInRole(Core.Interfaces.RoleNames.SuperAdmin)
            || httpContext.User.IsInRole(Core.Interfaces.RoleNames.Administrator);
    }
}
