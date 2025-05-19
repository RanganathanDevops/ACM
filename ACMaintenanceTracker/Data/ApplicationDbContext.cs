using Microsoft.EntityFrameworkCore;
using ACMaintenanceTracker.Models;

namespace ACMaintenanceTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ACUnit> ACUnits { get; set; }
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ACUnit>().ToTable("ACUnits");
            modelBuilder.Entity<MaintenanceRecord>().ToTable("MaintenanceRecords");

            modelBuilder.Entity<ACUnit>()
                .HasIndex(a => a.ACIdentifier)
                .IsUnique();
        }
    }
}