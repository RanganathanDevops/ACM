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
    public class ACUnitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ACUnitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ACUnits
        public async Task<IActionResult> Index()
        {
            return View(await _context.ACUnits.Include(a => a.MaintenanceRecords).ToListAsync());
        }

        // GET: ACUnits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aCUnit = await _context.ACUnits
                .Include(a => a.MaintenanceRecords)
                .FirstOrDefaultAsync(m => m.Id == id);
            var viewmodel = aCUnit;
            if (aCUnit == null)
            {
                return NotFound();
            }

            return View(viewmodel);
        }

        // GET: ACUnits/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ACUnits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ACUnitCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var aCUnit = new ACUnit
                {
                    ACIdentifier = viewModel.ACIdentifier,
                    FloorNumber = viewModel.FloorNumber,
                    RoomNumber = viewModel.RoomNumber,
                    Model = viewModel.Model,
                    InstallationDate = viewModel.InstallationDate.ToUniversalTime()
                };

                _context.Add(aCUnit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: ACUnits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aCUnit = await _context.ACUnits.FindAsync(id);
            if (aCUnit == null)
            {
                return NotFound();
            }
            return View(aCUnit);
        }

        // POST: ACUnits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ACUnitEditViewModel viewModel)
        {
            var aCUnit = new ACUnit
            {
                Id = viewModel.Id,
                ACIdentifier = viewModel.ACIdentifier,
                FloorNumber = viewModel.FloorNumber,
                RoomNumber = viewModel.RoomNumber,
                Model = viewModel.Model,
                InstallationDate = viewModel.InstallationDate.ToUniversalTime()
            };
            if (id != aCUnit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Update(aCUnit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ACUnitExists(aCUnit.Id))
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
            return View(aCUnit);
        }

        // GET: ACUnits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aCUnit = await _context.ACUnits
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aCUnit == null)
            {
                return NotFound();
            }

            return View(aCUnit);
        }

        // POST: ACUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aCUnit = await _context.ACUnits.FindAsync(id);
            _context.ACUnits.Remove(aCUnit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Generate QR Code for AC Unit
        public IActionResult GenerateQRCode(int id)
{
    var acUnit = _context.ACUnits.Find(id);
    if (acUnit == null)
    {
        return NotFound();
    }

    // Create QR code data - URL that will show AC info when scanned
    string qrCodeData = $"{Request.Scheme}://{Request.Host}/ACUnits/QRDetails/{acUnit.ACIdentifier}";

    QRCodeGenerator qrGenerator = new QRCodeGenerator();
    QRCodeData qrCodeDataObj = qrGenerator.CreateQrCode(qrCodeData, QRCodeGenerator.ECCLevel.Q);
    
    // Updated QRCode class usage
    using (var qrCode = new BitmapByteQRCode(qrCodeDataObj))
    {
        byte[] qrCodeImage = qrCode.GetGraphic(20);
        return File(qrCodeImage, "image/png");
    }
}

        // Show AC info when QR code is scanned
        public async Task<IActionResult> QRDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var aCUnit = await _context.ACUnits
                .Include(a => a.MaintenanceRecords)
                .FirstOrDefaultAsync(m => m.ACIdentifier == id);
            if (aCUnit == null)
            {
                return NotFound();
            }

            return View("QRDetails", aCUnit);
        }

        private bool ACUnitExists(int id)
        {
            return _context.ACUnits.Any(e => e.Id == id);
        }
    }
}