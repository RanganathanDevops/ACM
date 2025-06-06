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
        public DbSet<EBReading> EBReadings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Equipment>().ToTable("Equipment");
            modelBuilder.Entity<MaintenanceRecord>().ToTable("MaintenanceRecords");
            modelBuilder.Entity<EBReading>().ToTable("EBReadings");

            // Equipment configuration
            modelBuilder.Entity<Equipment>()
                .HasIndex(e => e.EquipmentIdentifier)
                .IsUnique();

            modelBuilder.Entity<Equipment>()
                .Property(e => e.EquipmentType)
                .HasConversion<string>();


            // Relationships
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.MaintenanceRecords)
                .WithOne(m => m.Equipment)
                .HasForeignKey(m => m.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.EBReadings)
                .WithOne(r => r.Equipment)
                .HasForeignKey(r => r.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // EBReading configuration
            modelBuilder.Entity<EBReading>()
                .Property(r => r.ReadingDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<EBReading>()
                .Property(r => r.RecordedBy)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}