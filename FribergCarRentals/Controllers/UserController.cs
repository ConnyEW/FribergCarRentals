using FribergCarRentals.Data.Repositories;
using FribergCarRentals.Enums;
using FribergCarRentals.Filters;
using FribergCarRentals.Models;
using FribergCarRentals.Services;
using FribergCarRentals.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace FribergCarRentals.Controllers
{
    [Route("Admin/User/[action]")]
    [AuthorizeAdmin]
    public class UserController : Controller
    {
        private readonly AdminService adminService;
        private readonly LoginService loginService;

        public UserController(AdminService adminService, LoginService loginService)
        {
            this.adminService = adminService;
            this.loginService = loginService;
        }

        // GET: UserController
        public async Task<IActionResult> IndexAsync(string? sortOrder = null)
        {
            var users = await adminService.GetAllUsersAsync(sortOrder);
            var usersVM = users.Select(u => new UserViewModel
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber
            }).ToList();

            return View(usersVM);
        }

        // GET: UserController/Details/5
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var user = await adminService.GetUserAsync(id);
            if (user == null) return RedirectToAction("Error", "Home");

            var userVM = new UserViewModel
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Password = user.Password,
                LastLogin = user.LastLogin,
                CreatedOn = user.CreatedOn,
                Rentals = user.Rentals.Select(r => new RentalViewModel
                {
                    CarName = r.Car.Name,
                    Price = r.Price,
                    IsRentalComplete = r.IsRentalComplete,
                    CompletionDate = r.CompletionDate,
                    RentalStart = r.RentalStart,
                    RentalEnd = r.RentalEnd,
                    RentalStatus = r.RentalStatus
                }).ToList() ?? new List<RentalViewModel>()
            };
            return View(userVM);
        }

        // GET: UserController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(UserViewModel userVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid input(s).");
                return View(userVM);
            }
            if (await loginService.EmailInUseAsync(userVM.Email))
            {
                ModelState.AddModelError("Email", "Email is already in use.");
                return View(userVM);
            }
            try
            {
                var user = await adminService.CreateUserAsync(userVM);
                TempData["ToastMessage"] = $"User '{user.Email}' was successfully created.";
                TempData["ToastClass"] = "positive";
                return RedirectToAction("Details", new { id = user.UserId });
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: UserController/Edit/5
        public async Task<IActionResult> EditAsync(int id)
        {
            var user = await adminService.GetUserAsync(id);
            if (user == null) return RedirectToAction("Error", "Home");

            var userVM = new UserViewModel
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
            return View(userVM);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(UserViewModel userVM)
        {
            var user = await adminService.GetUserAsync(userVM.UserId);
            if (user == null) return RedirectToAction("Error", "Home");

            // Password should not be editable by admins but should still be [Required] for use in other
            // actions. Remove validation for it so it can remain unchanged and still pass validation.
            ModelState.Remove("Password");
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid input.");
                return View(userVM);
            }
            user.FirstName = userVM.FirstName;
            user.LastName = userVM.LastName;
            user.PhoneNumber = userVM.PhoneNumber;
            try
            {
                await adminService.UpdateUserAsync(user);
                TempData["ToastMessage"] = $"User '{user.Email}' was successfully updated.";
                TempData["ToastClass"] = "positive";
                return RedirectToAction("Details", new { id = user.UserId });
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: UserController/Delete/5
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var user = await adminService.GetUserAsync(id);
            if (user == null) return RedirectToAction("Error", "Home");

            var userVM = new UserViewModel
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                LastLogin = user.LastLogin,
                CreatedOn = user.CreatedOn
            };
            return View(userVM);
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(UserViewModel userVM)
        {
            var user = await adminService.GetUserAsync(userVM.UserId);
            if (user == null) return RedirectToAction("Error", "Home");
            try
            {
                await adminService.DeleteUserAsync(user);
                TempData["ToastMessage"] = $"User '{user.Email}' was successfully deleted.";
                TempData["ToastClass"] = "neutral";
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
