using ACMaintenanceTracker.Data;
using ACMaintenanceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ACMaintenanceTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create a ViewModel class in your Models folder
        public class HomeViewModel
        {
            public List<MaintenanceRecord> RecentMaintenance { get; set; }
            public List<ACUnit> DueForMaintenance { get; set; }
        }

        // Update your HomeController
        public async Task<IActionResult> Index()
        {
            try
            {

                var todayUtc = DateTime.UtcNow.Date;
                var thirtyDaysFromNowUtc = todayUtc.AddDays(30);

                var recentMaintenance = await _context.MaintenanceRecords
                    .Include(m => m.ACUnit)
                    .OrderByDescending(m => m.MaintenanceDate)
                    .Take(5)
                    .ToListAsync();

                var dueForMaintenance = await _context.ACUnits
                    .Include(a => a.MaintenanceRecords)
                    .Where(a => a.MaintenanceRecords.Any(m =>
                        m.NextMaintenanceDate.HasValue &&
                        m.NextMaintenanceDate.Value.Date <= thirtyDaysFromNowUtc))
                    .ToListAsync();

                var viewModel = new HomeViewModel
                {
                    RecentMaintenance = recentMaintenance,
                    DueForMaintenance = dueForMaintenance
                };

                //ViewBag.RecentMaintenance = recentMaintenance;
                //ViewBag.DueForMaintenance = dueForMaintenance;

                //return View();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the error or return a meaningful error message
                return Content($"An error occurred: {ex.Message}");
            }
        }
    }
}