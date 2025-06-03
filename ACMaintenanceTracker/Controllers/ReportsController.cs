using ACMaintenanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACMaintenanceTracker.Controllers
{
    public class ReportsController : Controller
    {
        // Mock data - replace with your actual data access
        private static List<MaintenanceRecord> _mockRecords = new List<MaintenanceRecord>
        {
            new MaintenanceRecord
            {
                Id = 1,
                MaintenanceType = "Preventive",
                MaintenanceDate = DateTime.Now.AddDays(-30),
                NextMaintenanceDate = DateTime.Now.AddDays(30),
                Cost = 1200.50m,
                Equipment = new Equipment { EquipmentType = EquipmentType.Transformer, EquipmentIdentifier = "TR-001" }
            },
            new MaintenanceRecord
            {
                Id = 2,
                MaintenanceType = "Corrective",
                MaintenanceDate = DateTime.Now.AddDays(-15),
                NextMaintenanceDate = DateTime.Now.AddDays(90),
                Cost = 850.75m,
                Equipment = new Equipment { EquipmentType = EquipmentType.Generator, EquipmentIdentifier = "GEN-002" }
            },
            // Add more mock records as needed
        };

        public IActionResult Index()
        {
            var model = new ReportsViewModel
            {
                MaintenanceRecords = _mockRecords,
                StartDate = DateTime.Now.AddMonths(-1),
                EndDate = DateTime.Now,
                EquipmentTypes = Enum.GetValues(typeof(EquipmentType)).Cast<EquipmentType>().ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult GenerateReport(ReportsViewModel model)
        {
            // In a real application, you would filter records based on the model parameters
            model.MaintenanceRecords = _mockRecords
                .Where(r => r.MaintenanceDate >= model.StartDate && r.MaintenanceDate <= model.EndDate)
                .ToList();

            model.EquipmentTypes = Enum.GetValues(typeof(EquipmentType)).Cast<EquipmentType>().ToList();
            return View("Index", model);
        }
    }
}