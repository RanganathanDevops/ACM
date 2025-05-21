using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACMaintenanceTracker.Models
{
    public class ACUnit
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "AC Identifier")]
        public string ACIdentifier { get; set; }

        [Required]
        [Display(Name = "Floor Number")]
        public int FloorNumber { get; set; }

        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }

        public string Model { get; set; }

        [Display(Name = "Installation Date")]
        [DataType(DataType.Date)]
        public DateTime? InstallationDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<MaintenanceRecord> MaintenanceRecords { get; set; }
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

        public int ACUnitId { get; set; }
        public ACUnit ACUnit { get; set; }
    }

    public class ACUnitCreateViewModel
    {
        public string ACIdentifier { get; set; }
        public int FloorNumber { get; set; }
        public string RoomNumber { get; set; }
        public string Model { get; set; }
        public DateTime InstallationDate { get; set; }
    }

    public class ACUnitEditViewModel
    {
        public int Id { get; set; }
        public string ACIdentifier { get; set; }
        public int FloorNumber { get; set; }
        public string RoomNumber { get; set; }
        public string Model { get; set; }
        public DateTime InstallationDate { get; set; }
    }
}