using Ma.Web.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ma.Web
{
    /// <summary>
    /// Represents the extensions for data EF migrations. 
    /// </summary>
    public static class MigrationExtensions
    {
        /// <summary>
        /// Default roles for authorization used on page.
        /// </summary>
        private static readonly string[] DefaultRoles = new string[] { "Admin", "Subscriber" };
                      
        /// <summary>
        /// Migrate pending database changes, create default roles and add super user if not exists. 
        /// </summary>
        public static void AutoMigrateDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

                if (dbContext.Database.GetPendingMigrations().Any())
                {
                    Policy
                        .Handle<Exception>()
                        .WaitAndRetry(10, r => TimeSpan.FromSeconds(10))
                        .Execute(() => dbContext.Database.Migrate());
                    
                    AddDefaultAdminAsync(scope).Wait();
                }
            }
        }

        private static async Task AddDefaultAdminAsync(IServiceScope scope)
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in DefaultRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role)); // Creates default roles
                }
            }

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var email = configuration["Admin:Email"];

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var superUser = new IdentityUser
                {
                    //Id             = Guid.NewGuid().ToString(),
                    Email          = email,
                    UserName       = email,
                    EmailConfirmed = true
                };

                var createPowerUser = await userManager.CreateAsync(superUser, configuration["Admin:Password"]);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(superUser, DefaultRoles[0]);
                    // NOTE: It is important to add role claim for the super user. 
                    // Only in this way does attribute [Authorize] work.
                    await userManager.AddClaimAsync(superUser, new Claim(ClaimTypes.Role, DefaultRoles[0]));
                }
            }
        }
    }
}
