using ACMaintenanceTracker.Data;
using ACMaintenanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ACMaintenanceTracker.Controllers
{
    public class MaintenanceRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MaintenanceRecords/Create
        public IActionResult Create(int equipmentId)
        {
            var equipment = _context.Equipment.Find(equipmentId);
            if (equipment == null)
            {
                return NotFound();
            }

            ViewData["EquipmentId"] = equipmentId;
            ViewData["EquipmentIdentifier"] = equipment.EquipmentIdentifier;
            return View();
        }

        // POST: MaintenanceRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMaintenanceRecord maintenanceRecord)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var record = new MaintenanceRecord
                    {
                        EquipmentId = maintenanceRecord.EquipmentId,
                        MaintenanceType = maintenanceRecord.MaintenanceType,
                        TechnicianName = maintenanceRecord.TechnicianName,
                        Notes = maintenanceRecord.Notes ?? string.Empty,
                        MaintenanceDate = DateTime.SpecifyKind(
                            maintenanceRecord.MaintenanceDate,
                            DateTimeKind.Utc),
                        NextMaintenanceDate = maintenanceRecord.NextMaintenanceDate.HasValue ?
                            DateTime.SpecifyKind(maintenanceRecord.NextMaintenanceDate.Value, DateTimeKind.Utc) :
                            (DateTime?)null,
                        PartsReplaced = maintenanceRecord.PartsReplaced ?? string.Empty, 
                        Cost = maintenanceRecord.Cost
                    };

                    _context.Add(record);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Equipment", new { id = maintenanceRecord.EquipmentId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the record.");
                    // Consider logging the exception (ex) here for debugging
                }
            }

            var equipment = _context.Equipment.Find(maintenanceRecord.EquipmentId);
            ViewData["EquipmentIdentifier"] = equipment?.EquipmentIdentifier;
            return View(maintenanceRecord);
        }

        private bool MaintenanceRecordExists(int id)
        {
            return _context.MaintenanceRecords.Any(e => e.Id == id);
        }
    }
}