using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Ma.Web.Areas.Identity.IdentityHostingStartup))]
namespace Ma.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}