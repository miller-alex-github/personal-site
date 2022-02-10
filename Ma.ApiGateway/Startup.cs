using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Ma.ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            if (Env.EnvironmentName == "Development")
                services.AddOcelot(Configuration);
            else
                services.AddOcelot(Configuration).AddDelegatingHandler<ProxyHandler>(true); // Use proxy in production
        }
                
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
                app.UseDeveloperExceptionPage();
            
            await app.UseOcelot();
        }
    }
}
