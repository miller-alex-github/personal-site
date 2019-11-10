using Microsoft.EntityFrameworkCore;

namespace Ma.Services.Appointments
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<AppointmentItem> Appointments { get; set; }
    }
}
