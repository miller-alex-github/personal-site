using Hangfire;
using Ma.Web.Data;
using Ma.Web.Filters;
using Ma.Web.HangfireJobs;
using Ma.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Ma.Web
{
    public class Startup
    {
        // Check daily for appointment notification.
        private readonly static string HANGFIRE_PERIODE = Cron.Daily(); 
       
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IAppointmentNotification, AppointmentNotification>();
            services.AddHangfire(configuration => configuration.UseSqlServerStorage(connectionString));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // auto migrate db
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<ApplicationDbContext>().MigrateDB();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            // Enabled Hangfire server with dashboard and start a daily job to check the appointments.            
            app.UseHangfireServer();
            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }                
            });
            RecurringJob.AddOrUpdate<IAppointmentNotification>(appointment => appointment.Check(), HANGFIRE_PERIODE);
                       
            app.Map("/ping", Ping);

            // Make life a little harder for the hacker.
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "DENY");           // Protect against clickjacking
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff"); // Prevents the browser from doing MIME-type sniffing
                context.Response.Headers.Add("X-Xss-Protection", "1");             // Prevent XSS attacks
                                
                await next();                
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// If we use IIS service on Shared Hosting then we have some problem with the Hangfire jobs.
        /// It will not be run correct because after expired Idle-Timeout the IIS will shutdown our page.        
        /// To trigger the Idle Timeout of IIS application pool I use a external server which will send 
        /// in defined period a HTTP request to this server. As a result the Hangfire can run his 
        /// periodically jobs without interrupts.
        /// I know this solution is not perfect but I didn't find a better one. 
        /// It works as follow:
        ///  1. The external server sends HTTP GET request to this server (url/ping).
        ///  2. This server answer with expected next duration for ping.
        ///  3. The external server parse the response and sends next ping request as defined.
        /// </summary>       
        private static void Ping(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync(HANGFIRE_PERIODE);
            });
        }
    }
}
