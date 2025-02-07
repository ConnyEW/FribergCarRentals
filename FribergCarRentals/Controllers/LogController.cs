using FribergCarRentals.Filters;
using FribergCarRentals.Services;
using FribergCarRentals.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentals.Controllers
{
    [Route("Admin/Log/[action]")]
    [AuthorizeAdmin]
    public class LogController : Controller
    {
        private readonly AdminService adminService;

        public LogController(AdminService adminService)
        {
            this.adminService = adminService;
        }
        // GET: LogController
        public async Task<IActionResult> IndexAsync(string? sortOrder = "dateDesc")
        {
            var logs = await adminService.GetAllLogsAsync(sortOrder);
            var logsVM = logs.Select(l => new LogViewModel
            {
                LogId = l.LogId,
                LogText = l.LogText,
                LogDate = l.LogDate
            });
            return View(logsVM);
        }
    }
}
