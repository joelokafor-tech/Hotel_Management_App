using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myHotel.Infrastructure;

namespace myHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuditLogsController : Controller
    {
        private readonly Hotel_Management_Context _context;

        public AuditLogsController(Hotel_Management_Context context)
        {
            _context = context;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var logs = await _context.AuditLogs
        //        .OrderByDescending(l => l.Timestamp)
        //        .ToListAsync();

        //    return View(logs);
        //}

        public IActionResult Index(string performedBy, string actionType, DateTime? startDate, DateTime? endDate)
        {
            var logs = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(performedBy))
            {
                logs = logs.Where(l => l.PerformedBy.Contains(performedBy));
            }

            if (!string.IsNullOrEmpty(actionType))
            {
                logs = logs.Where(l => l.Action == actionType);
            }

            if (startDate.HasValue)
            {
                logs = logs.Where(l => l.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                logs = logs.Where(l => l.Timestamp <= endDate.Value);
            }

            var result = logs
                .OrderByDescending(l => l.Timestamp)
                .ToList();

            // Get distinct action values for dropdown
            var actionTypes = _context.AuditLogs
                .Select(l => l.Action)
                .Distinct()
                .OrderBy(a => a)
                .ToList();

            // Pass filter values to the view
            ViewData["PerformedBy"] = performedBy;
            ViewData["ActionType"] = actionType;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewData["ActionTypes"] = actionTypes;

            return View(result);
        }
    }
}
