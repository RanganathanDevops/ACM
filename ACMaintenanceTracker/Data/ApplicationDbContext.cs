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

        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Equipment>().ToTable("Equipment");
            modelBuilder.Entity<MaintenanceRecord>().ToTable("MaintenanceRecords");

            modelBuilder.Entity<Equipment>()
                .HasIndex(e => e.EquipmentIdentifier)
                .IsUnique();

            modelBuilder.Entity<Equipment>()
                .Property(e => e.EquipmentType)
                .HasConversion<string>();

            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.MaintenanceRecords)
                .WithOne(m => m.Equipment)
                .HasForeignKey(m => m.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}