using ACMaintenanceTracker.Data;
using ACMaintenanceTracker.Models;
using iTextSharp.text.pdf.qrcode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace ACMaintenanceTracker.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EquipmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Equipment
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Equipment
        //        .Include(e => e.MaintenanceRecords)
        //        .OrderBy(e => e.EquipmentType)
        //        .ThenBy(e => e.EquipmentIdentifier)
        //        .ToListAsync());
        //}
        public async Task<IActionResult> Index(string type)
        {
            IQueryable<Equipment> query = _context.Equipment
                .Include(e => e.MaintenanceRecords)
                .OrderBy(e => e.EquipmentType)
                .ThenBy(e => e.EquipmentIdentifier);

            // Filter by equipment type if provided
            if (!string.IsNullOrEmpty(type))
            {
                if (Enum.TryParse<EquipmentType>(type, out var typeFilter))
                {
                    query = query.Where(e => e.EquipmentType == typeFilter);
                }
            }

            var equipmentList = await query.ToListAsync();

            // Pass the current filter to the view
            ViewData["CurrentFilter"] = type;

            return View(equipmentList);
        }

        // GET: Equipment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment
                .Include(e => e.MaintenanceRecords)
                .Include(e => e.ConnectedOutdoorUnit)
                .Include(e => e.ConnectedIndoorUnits)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // GET: Equipment/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Equipment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EquipmentCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var equipment = new Equipment
                {
                    EquipmentIdentifier = viewModel.EquipmentIdentifier,
                    EquipmentType = viewModel.EquipmentType,
                    Location = viewModel.Location,
                    Model = viewModel.Model,
                    Manufacturer = viewModel.Manufacturer,
                    InstallationDate = viewModel.InstallationDate.ToUniversalTime(),
                    //Capacity = viewModel.Capacity
                };

                _context.Add(equipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Equipment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            var viewModel = new EquipmentEditViewModel
            {
                Id = equipment.Id,
                EquipmentIdentifier = equipment.EquipmentIdentifier,
                EquipmentType = equipment.EquipmentType,
                Location = equipment.Location,
                Model = equipment.Model,
                Manufacturer = equipment.Manufacturer,
                InstallationDate = equipment.InstallationDate ?? DateTime.UtcNow,
                //Capacity = equipment.Capacity
            };

            return View(viewModel);
        }

        // POST: Equipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EquipmentEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var equipment = new Equipment
                    {
                        Id = viewModel.Id,
                        EquipmentIdentifier = viewModel.EquipmentIdentifier,
                        EquipmentType = viewModel.EquipmentType,
                        Location = viewModel.Location,
                        Model = viewModel.Model,
                        Manufacturer = viewModel.Manufacturer,
                        InstallationDate = viewModel.InstallationDate.ToUniversalTime(),
                        //Capacity = viewModel.Capacity
                    };

                    _context.Update(equipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipmentExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Equipment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // POST: Equipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            _context.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Generate QR Code for Equipment
        public IActionResult GenerateQRCode(int id)
        {
            var equipment = _context.Equipment.Find(id);
            if (equipment == null)
            {
                return NotFound();
            }

            string qrCodeData = $"{Request.Scheme}://{Request.Host}/Equipment/QRDetails/{equipment.EquipmentIdentifier}";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeDataObj = qrGenerator.CreateQrCode(qrCodeData, QRCodeGenerator.ECCLevel.Q);

            using (var qrCode = new BitmapByteQRCode(qrCodeDataObj))
            {
                byte[] qrCodeImage = qrCode.GetGraphic(20);
                return File(qrCodeImage, "image/png");
            }
        }

        // Show Equipment info when QR code is scanned
        public async Task<IActionResult> QRDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var equipment = await _context.Equipment
                .Include(e => e.MaintenanceRecords)
                .FirstOrDefaultAsync(m => m.EquipmentIdentifier == id);
            if (equipment == null)
            {
                return NotFound();
            }

            return View("QRDetails", equipment);
        }

        private bool EquipmentExists(int id)
        {
            return _context.Equipment.Any(e => e.Id == id);
        }
    }
}