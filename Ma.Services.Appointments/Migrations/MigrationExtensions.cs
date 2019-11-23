using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Linq;

namespace Ma.Services.Appointments
{
    /// <summary>
    /// Represents the extensions for data EF migrations. 
    /// </summary>
    public static class MigrationExtensions
    {
        /// <summary>
        /// Migrate pending database changes. 
        /// </summary>
        public static void AutoMigrateDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                if (context.Database.GetPendingMigrations().Any())
                {
                    Policy
                        .Handle<Exception>()
                        .WaitAndRetry(10, r => TimeSpan.FromSeconds(10))
                        .Execute(() => context.Database.Migrate());
                }
            }
        }
    }
}
