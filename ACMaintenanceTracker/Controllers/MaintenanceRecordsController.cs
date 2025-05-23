using ACMaintenanceTracker.Data;
using ACMaintenanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public IActionResult Create(int acUnitId)
        {
            var acUnit = _context.ACUnits.Find(acUnitId);
            //var acUnit = _context.ACUnits.Find(1);
            if (acUnit == null)
            {
                return NotFound();
            }

            ViewData["ACUnitId"] = acUnitId;
            ViewData["ACIdentifier"] = acUnit.ACIdentifier;
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
                        ACUnitId = maintenanceRecord.ACUnitId,
                        MaintenanceType = maintenanceRecord.MaintenanceType,
                        TechnicianName = maintenanceRecord.TechnicianName,
                        Notes = maintenanceRecord.Notes,
                        MaintenanceDate = DateTime.SpecifyKind(
                            maintenanceRecord.MaintenanceDate,
                            DateTimeKind.Utc),
                        NextMaintenanceDate = DateTime.SpecifyKind(
                            maintenanceRecord.NextMaintenanceDate,
                            DateTimeKind.Utc)
                    };


                    _context.Add(record);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "ACUnits", new { id = maintenanceRecord.ACUnitId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the record.");
                }
            }

            var unit = _context.ACUnits.Find(maintenanceRecord.ACUnitId);
            ViewData["ACIdentifier"] = unit?.ACIdentifier;
            return View(maintenanceRecord);
        }


        //// GET: MaintenanceRecords/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var maintenanceRecord = await _context.MaintenanceRecords.FindAsync(id);
        //    if (maintenanceRecord == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ACUnitId"] = maintenanceRecord.ACUnitId;
        //    ViewData["ACIdentifier"] = _context.ACUnits.Find(maintenanceRecord.ACUnitId)?.ACIdentifier;
        //    return View(maintenanceRecord);
        //}

        //// POST: MaintenanceRecords/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,ACUnitId,MaintenanceDate,MaintenanceType,TechnicianName,Notes,NextMaintenanceDate")] MaintenanceRecord maintenanceRecord)
        //{
        //    if (id != maintenanceRecord.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(maintenanceRecord);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!MaintenanceRecordExists(maintenanceRecord.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction("Details", "ACUnits", new { id = maintenanceRecord.ACUnitId });
        //    }
        //    ViewData["ACUnitId"] = maintenanceRecord.ACUnitId;
        //    ViewData["ACIdentifier"] = _context.ACUnits.Find(maintenanceRecord.ACUnitId)?.ACIdentifier;
        //    return View(maintenanceRecord);
        //}

        //// GET: MaintenanceRecords/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var maintenanceRecord = await _context.MaintenanceRecords
        //        .Include(m => m.ACUnit)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (maintenanceRecord == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(maintenanceRecord);
        //}

        //// POST: MaintenanceRecords/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var maintenanceRecord = await _context.MaintenanceRecords.FindAsync(id);
        //    int acUnitId = maintenanceRecord.ACUnitId;
        //    _context.MaintenanceRecords.Remove(maintenanceRecord);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("Details", "ACUnits", new { id = acUnitId });
        //}

        private bool MaintenanceRecordExists(int id)
        {
            return _context.MaintenanceRecords.Any(e => e.Id == id);
        }
    }
}