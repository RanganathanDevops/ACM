using ACMaintenanceTracker.Data;
using ACMaintenanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ACMaintenanceTracker.Controllers
{
    public class EBReadingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EBReadingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EBReadings
        public async Task<IActionResult> Index()
        {
            var readings = await _context.EBReadings
                .Include(r => r.Equipment)
                .OrderByDescending(r => r.ReadingDate)
                .ToListAsync();

            return View(readings);
        }

        // GET: EBReadings/Create
        public async Task<IActionResult> Create()
        {
            var equipmentList = await _context.Equipment
                .OrderBy(e => e.EquipmentIdentifier)
                .ToListAsync();

            var model = new EBReadingViewModel
            {
                EquipmentList = equipmentList,
                ReadingDate = DateTime.Now,
                RecordedBy = User.Identity?.Name ?? "System"
            };

            return View(model);
        }

        // POST: EBReadings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EBReading viewModel)
        {
            if (ModelState.IsValid)
            {
                var reading = new EBReading
                {
                    EquipmentId = viewModel.EquipmentId,
                    ReadingDate = viewModel.ReadingDate,
                    PreviousReading = viewModel.PreviousReading,
                    CurrentReading = viewModel.CurrentReading,
                    RatePerUnit = viewModel.RatePerUnit,
                    Notes = viewModel.Notes,
                    RecordedBy = viewModel.RecordedBy,
                    UnitsConsumed = viewModel.CurrentReading - viewModel.PreviousReading,
                    TotalCost = (viewModel.CurrentReading - viewModel.PreviousReading) * viewModel.RatePerUnit,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Add(reading);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "EB Reading added successfully!";
                return RedirectToAction(nameof(Index));
            }

            // If we got here, something went wrong
            //viewModel.EquipmentList = await _context.Equipment
            //    .OrderBy(e => e.EquipmentIdentifier)
            //    .ToListAsync();

            return View(viewModel);
        }

        // AJAX: Get previous reading for equipment
        [HttpGet]
        public async Task<JsonResult> GetPreviousReading(int equipmentId)
        {
            var lastReading = await _context.EBReadings
                .Where(r => r.EquipmentId == equipmentId)
                .OrderByDescending(r => r.ReadingDate)
                .Select(r => r.CurrentReading)
                .FirstOrDefaultAsync();

            return Json(new { previousReading = lastReading });
        }
    }
}