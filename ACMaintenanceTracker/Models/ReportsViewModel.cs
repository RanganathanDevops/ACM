using ACMaintenanceTracker.Models;
using System;
using System.Collections.Generic;

namespace ACMaintenanceTracker.Models
{
    public class ReportsViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SelectedEquipmentType { get; set; }
        public List<EquipmentType> EquipmentTypes { get; set; }
        public List<MaintenanceRecord> MaintenanceRecords { get; set; }
    }
}