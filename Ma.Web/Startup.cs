﻿using Hangfire;
using Ma.Web.Data;
using Ma.Web.Filters;
using Ma.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Ma.Web
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

            services.AddDataProtection()
               .SetApplicationName("isolation-name")
               .PersistKeysToFileSystem(new DirectoryInfo("."));

            services.AddDefaultIdentity<IdentityUser>(config =>
                {
                    config.SignIn.RequireConfirmedEmail = true;
                })
                .AddRoles<IdentityRole>()
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IJsonWebTokenService, JsonWebTokenService>(); // JSON Web Token service           
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IAppointmentEmailNotification, AppointmentEmailNotification>();
            services.AddHangfire(configuration => configuration.UseSqlServerStorage(connectionString));

            // REST service 'Appointments'
            services.AddHttpClient<IAppointmentsAPI, AppointmentsAPI>("ApiGateway", c =>
            {
                c.BaseAddress = new Uri(Configuration["ApiGateway"]);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler()
            {
                Proxy = Env.EnvironmentName == "Development" ? null : new WebProxy("http://winproxy.server.lan:3128")   
            });

            services.AddRouting();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddHangfireServer(); // Enabled Hangfire server with dashboard and start a daily job to check the appointments.
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();                
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(configurer => configurer
                    .MaxAge(days: 365)                    
                    .IncludeSubdomains()
                    .Preload());
            }

            app.AutoMigrateDatabase();         
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }, 
                AppPath = "/Admin" // go back to admin site                
            });
            RecurringJob.AddOrUpdate<IAppointmentEmailNotification>(appointment => appointment.CheckAppointment(), Cron.Daily(9)); // 9 o'clock AM

            app.Map("/ping", Ping);

            // Make life a little harder for the hacker.
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "DENY");           // Protect against clickjacking attacks
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff"); // Prevents the browser from doing MIME-type sniffing
                context.Response.Headers.Add("X-Xss-Protection", "1");             // Prevents XSS attacks
                                
                await next();                
            });

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// If we use IIS service on Shared Hosting then we have some problem with the Hangfire jobs.
        /// It will not be run correct because after expired Idle-Timeout the IIS will shutdown our page.        
        /// To trigger the Idle Timeout of IIS application pool I use a external server which will send 
        /// in defined period a HTTP request to this server. As a result the Hangfire can run his 
        /// periodically jobs without interrupts.
        /// I know this solution is not perfect but I didn't find a better one for IIS. 
        /// It works as follow:
        ///  1. The external server sends HTTP GET request to this server (url/ping).
        ///  2. This server answer with expected interval which should be used for next ping.
        ///  3. The external server parse the response and sends next ping request as defined.
        /// </summary>       
        private static void Ping(IApplicationBuilder app)
        {
            app.Run(async context =>
            {                
                await context.Response.WriteAsync("1200"); // ping every 20 minutes
            });
        }
    }
}
