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

        public class HomeViewModel
        {
            public List<MaintenanceRecord> RecentMaintenance { get; set; }
            public List<Equipment> DueForMaintenance { get; set; }
            public Dictionary<EquipmentType, int> EquipmentCounts { get; set; }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var todayUtc = DateTime.UtcNow.Date;
                var thirtyDaysFromNowUtc = todayUtc.AddDays(30);

                var recentMaintenance = await _context.MaintenanceRecords
                    .Include(m => m.Equipment)
                    .OrderByDescending(m => m.MaintenanceDate)
                    .Take(5)
                    .ToListAsync();

                var dueForMaintenance = await _context.Equipment
                    .Include(e => e.MaintenanceRecords)
                    .Where(e => e.MaintenanceRecords.Any(m =>
                        m.NextMaintenanceDate.HasValue &&
                        m.NextMaintenanceDate.Value.Date <= thirtyDaysFromNowUtc))
                    .ToListAsync();

                var equipmentCounts = await _context.Equipment
                    .GroupBy(e => e.EquipmentType)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Type, x => x.Count);

                var viewModel = new HomeViewModel
                {
                    RecentMaintenance = recentMaintenance,
                    DueForMaintenance = dueForMaintenance,
                    EquipmentCounts = equipmentCounts
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return Content($"An error occurred: {ex.Message}");
            }
        }
    }
}