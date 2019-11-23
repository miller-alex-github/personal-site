using Microsoft.EntityFrameworkCore;
#pragma warning disable 1591

namespace Ma.Services.Appointments
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Appointment> Appointments { get; set; }
    }
}
