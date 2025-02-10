using FribergCarRentals.Filters;
using FribergCarRentals.Models;
using FribergCarRentals.Services;
using FribergCarRentals.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentals.Controllers
{
    public class AdminController : Controller
    {
        private readonly LoginService loginService;
        private readonly AdminService adminService;

        public AdminController(LoginService loginService, AdminService adminService)
        {
            this.loginService = loginService;
            this.adminService = adminService;
        }
        // GET
        public IActionResult Index()
        {
            // Redirect to admin dashboard if admin is already logged in
            if (HttpContext.Session.GetString("AdminId") != null) return RedirectToAction("Dashboard");
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogInAsync(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Login attempt failed.");
                return View("Index", loginVM);
            }

            if (await loginService.ValidateAdminLoginAsync(loginVM.Email, loginVM.Password))
            {
                // Fetch admin ID from email
                var admin = await loginService.GetAdminByEmailAsync(loginVM.Email);
                if (admin == null) return RedirectToAction("Error", "Home");
                loginVM.Email = admin.Email;
                loginVM.AdminId = admin.AdminId;

                // Set state variables
                HttpContext.Session.Remove("UserId");
                HttpContext.Session.Remove("UserEmail");
                HttpContext.Session.SetString("AdminEmail", loginVM.Email);
                HttpContext.Session.SetInt32("AdminId", loginVM.AdminId);
                return RedirectToAction("Dashboard");
            }
            ModelState.AddModelError("", "Login attempt failed.");
            return View("Index", loginVM);

        }
        [AuthorizeAdmin]
        public async Task<IActionResult> DashboardAsync()
        {
            ViewBag.Revenue = await adminService.TotalRevenueAsync();
            ViewBag.Users = await adminService.TotalUsersAsync();
            ViewBag.UpcomingRentals = await adminService.TotalUpcomingRentalsAsync();
            ViewBag.OngoingRentals = await adminService.TotalOngoingRentalsAsync();
            ViewBag.CompletedRentals = await adminService.TotalCompletedRentalsAsync();
            ViewBag.ActiveCars = await adminService.ActiveCarsAsync();
            return View();
        }

        [Route("Admin/Admins/[action]")]
        [AuthorizeAdmin]
        public async Task<IActionResult> ListAsync(string? sortOrder = null)
        {
            var admins = await adminService.GetAllAdminsAsync(sortOrder);
            var adminsVM = admins.Select(a => new AdminViewModel
            {
                AdminId = a.AdminId,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
                IsSuperAdmin = a.IsSuperAdmin
            }).ToList();

            return View(adminsVM);
        }

        [Route("Admin/Admins/[action]")]
        [AuthorizeAdmin]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var admin = await adminService.GetAdminAsync(id);
            if (admin == null) return RedirectToAction("Error", "Home");

            var adminVM = new AdminViewModel
            {
                AdminId = admin.AdminId,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.Email,
                IsSuperAdmin = admin.IsSuperAdmin
            };
            return View(adminVM);
        }

        [Route("Admin/Admins/[action]")]
        [AuthorizeAdmin]
        public async Task<IActionResult> CreateAsync()
        {
            var id = HttpContext.Session.GetInt32("AdminId") ?? 0;
            var admin = await adminService.GetAdminAsync(id);
            if (admin == null) return RedirectToAction("Error", "Home");

            if (!admin.IsSuperAdmin)
            {
                TempData["ToastMessage"] = "Only SuperAdmins can create new admins.";
                TempData["ToastClass"] = "negative";
                return RedirectToAction("List");
            }

            return View();
        }

        [Route("Admin/Admins/[action]")]
        [AuthorizeAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(AdminViewModel adminVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid input(s).");
                return View(adminVM);
            }
            if (await loginService.EmailInUseAsync(adminVM.Email))
            {
                ModelState.AddModelError("Email", "Email is already in use.");
                return View(adminVM);
            }
            try
            {
                var admin = await adminService.CreateAdminAsync(adminVM);
                TempData["ToastMessage"] = $"Admin '{admin.Email}' was successfully created.";
                TempData["ToastClass"] = "positive";
                return RedirectToAction("Details", new { id = admin.AdminId });
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [Route("Admin/Admins/[action]")]
        [AuthorizeAdmin]
        public async Task<IActionResult> EditAsync(int id)
        {
            var admin = await adminService.GetAdminAsync(id);
            var sessionId = HttpContext.Session.GetInt32("AdminId") ?? 0;
            var sessionAdmin = await adminService.GetAdminAsync(sessionId);
            if (admin == null || sessionAdmin == null) return RedirectToAction("Error", "Home");

            if (!sessionAdmin.IsSuperAdmin)
            {
                TempData["ToastMessage"] = "Only SuperAdmins can edit admins.";
                TempData["ToastClass"] = "negative";
                return RedirectToAction("List");
            }

            var adminVM = new AdminViewModel
            {
                AdminId = admin.AdminId,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.Email
            };
            return View(adminVM);
        }

        [Route("Admin/Admins/[action]")]
        [AuthorizeAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(AdminViewModel adminVM)
        {
            // Password should not be editable by admins but should still be [Required] for use in other
            // actions. Remove validation for it so it can remain unchanged and still pass validation.
            ModelState.Remove("Password");

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid input.");
                return View(adminVM);
            }
            var admin = await adminService.GetAdminAsync(adminVM.AdminId);
            if (admin == null) return RedirectToAction("Error", "Home");

            admin.FirstName = adminVM.FirstName;
            admin.LastName = adminVM.LastName;

            try
            {
                await adminService.UpdateAdminAsync(admin);
                TempData["ToastMessage"] = $"Admin '{admin.Email}' was successfully updated.";
                TempData["ToastClass"] = "positive";
                return RedirectToAction("Details", new { id = admin.AdminId });
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [Route("Admin/Admins/[action]")]
        [AuthorizeAdmin]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var admin = await adminService.GetAdminAsync(id);
            var sessionAdmin = await adminService.GetAdminAsync(HttpContext.Session.GetInt32("AdminId") ?? 0);


            if (admin == null || sessionAdmin == null) return RedirectToAction("Error", "Home");

            if (admin.IsSuperAdmin)
            {
                TempData["ToastMessage"] = "SuperAdmins cannot be deleted.";
                TempData["ToastClass"] = "negative";
                return RedirectToAction("Details", new { id });
            }
            if (!sessionAdmin.IsSuperAdmin)
            {
                TempData["ToastMessage"] = "Only SuperAdmins can delete admins.";
                TempData["ToastClass"] = "negative";
                return RedirectToAction("Details", new { id });
            }

            var adminVM = new AdminViewModel
            {
                AdminId = admin.AdminId,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.Email
            };
            return View(adminVM);
        }

        [Route("Admin/Admins/[action]")]
        [AuthorizeAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(AdminViewModel adminVM)
        {
            var admin = await adminService.GetAdminAsync(adminVM.AdminId);
            if (admin == null) return RedirectToAction("Error", "Home");

            try
            {
                await adminService.DeleteAdminAsync(admin);
                TempData["ToastMessage"] = $"Admin '{admin.Email}' was successfully deleted.";
                TempData["ToastClass"] = "neutral";
                return RedirectToAction("List");
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
