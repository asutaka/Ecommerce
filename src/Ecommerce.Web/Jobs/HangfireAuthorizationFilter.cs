using Hangfire.Dashboard;

namespace Ecommerce.Web.Jobs;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // In production, add proper authorization check
        // For now, allow access in development
        return true;
        
        // Example for production:
        // var httpContext = context.GetHttpContext();
        // return httpContext.User.Identity?.IsAuthenticated == true && 
        //        httpContext.User.IsInRole("Admin");
    }
}
