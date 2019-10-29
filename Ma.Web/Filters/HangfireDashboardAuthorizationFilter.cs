using Hangfire.Dashboard;

namespace Ma.Web.Filters
{
    /// <summary>
    /// Hangfire Dashboard exposes sensitive information about the background jobs. 
    /// We will restrict access to the Dashboard with our own filter.
    /// </summary>
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // TODO: Hangfire -> httpContext.User.IsInRole("Admin")
            
            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return httpContext.User.Identity.IsAuthenticated;
        }
    }
}
