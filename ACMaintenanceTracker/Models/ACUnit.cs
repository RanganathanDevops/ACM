using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACMaintenanceTracker.Models
{
    public enum EquipmentType
    {
        Transformer,
        Generator,
        WaterPlant,
        ROPlant,
        STPlant,
        FireSystem,
        ACIndoorUnit,
        ACOutdoorUnit
    }

    public class Equipment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Equipment Identifier")]
        public string EquipmentIdentifier { get; set; }

        [Required]
        [Display(Name = "Equipment Type")]
        public EquipmentType EquipmentType { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Model")]
        public string Model { get; set; }

        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }

        [Display(Name = "Installation Date")]
        [DataType(DataType.Date)]
        public DateTime? InstallationDate { get; set; }

        [Display(Name = "Next Service Date")]
        [DataType(DataType.Date)]
        public DateTime? NextServiceDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //[Display(Name = "Is Active")]
        //public bool IsActive { get; set; } = true;

        // Navigation properties
        public List<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
        public List<EBReading> EBReadings { get; set; } = new List<EBReading>();

        // For AC Indoor Units - reference to connected outdoor unit
        [Display(Name = "Connected Outdoor Unit")]
        [ForeignKey("ConnectedOutdoorUnitId")]
        public Equipment ConnectedOutdoorUnit { get; set; }
        public int? ConnectedOutdoorUnitId { get; set; }

        // For AC Outdoor Units - collection of connected indoor units
        [InverseProperty("ConnectedOutdoorUnit")]
        public ICollection<Equipment> ConnectedIndoorUnits { get; set; }
    }

    public class MaintenanceRecord
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Maintenance Date")]
        [DataType(DataType.Date)]
        public DateTime MaintenanceDate { get; set; }

        [Required]
        [Display(Name = "Maintenance Type")]
        public string MaintenanceType { get; set; }

        [Display(Name = "Technician Name")]
        public string TechnicianName { get; set; }

        public string Notes { get; set; }

        [Display(Name = "Next Maintenance Date")]
        [DataType(DataType.Date)]
        public DateTime? NextMaintenanceDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        [Display(Name = "Parts Replaced")]
        public string? PartsReplaced { get; set; }

        [Display(Name = "Cost")]
        public decimal? Cost { get; set; }
    }

    public class EBReading
    {
        public int Id { get; set; }

        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        [Display(Name = "Reading Date")]
        public DateTime ReadingDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Previous Reading")]
        public decimal PreviousReading { get; set; }

        [Display(Name = "Current Reading")]
        public decimal CurrentReading { get; set; }

        [Display(Name = "Units Consumed")]
        public decimal UnitsConsumed { get; set; }

        [Display(Name = "Rate Per Unit")]
        public decimal RatePerUnit { get; set; }

        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }

        public string Notes { get; set; }

        [Required]
        [Display(Name = "Recorded By")]
        public string RecordedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class EquipmentCreateViewModel
    {
        [Required]
        public string EquipmentIdentifier { get; set; }

        [Required]
        public EquipmentType EquipmentType { get; set; }

        public string Location { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public DateTime InstallationDate { get; set; }
    }

    public class EquipmentEditViewModel
    {
        public int Id { get; set; }
        public string EquipmentIdentifier { get; set; }
        public EquipmentType EquipmentType { get; set; }
        public string Location { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public DateTime InstallationDate { get; set; }
    }

    public class CreateMaintenanceRecord
    {
        [Required]
        public string MaintenanceType { get; set; }

        [Required]
        public int EquipmentId { get; set; }

        public string TechnicianName { get; set; }
        public string Notes { get; set; }

        [Required]
        public DateTime MaintenanceDate { get; set; }

        public DateTime? NextMaintenanceDate { get; set; }
        public string? PartsReplaced { get; set; }
        public decimal? Cost { get; set; }
    }

    public class EBReadingViewModel
    {
        public int EquipmentId { get; set; }
        public DateTime ReadingDate { get; set; }
        public decimal PreviousReading { get; set; }
        public decimal CurrentReading { get; set; }
        public decimal RatePerUnit { get; set; }
        public string Notes { get; set; }
        public string RecordedBy { get; set; }

        // For dropdown list
        public List<Equipment> EquipmentList { get; set; }
    }
}