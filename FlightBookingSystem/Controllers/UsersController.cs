using System.Security.Claims;
using FlightBookingSystem.Models;
using FlightBookingSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBookingSystem.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            userId = 0;
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out userId);
        }

        // GET /Users/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var vm = await _userService.GetDashboardAsync(userId);
            if (vm == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            return View(vm);
        }

        // GET /Users/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                _logger.LogWarning("Profile accessed without a valid NameIdentifier claim.");
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var profile = await _userService.GetProfileAsync(userId);
            if (profile == null)
            {
                _logger.LogWarning("Profile not found for UserId={UserId} from valid cookie. Signing out.", userId);
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            return View(profile);
        }

        // GET /Users/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Forbid();

            var model = await _userService.GetEditModelAsync(userId);
            if (model == null) return NotFound();

            return View(model);
        }

        // POST /Users/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Forbid();

            if (!ModelState.IsValid) return View(model);

            var ok = await _userService.UpdateProfileAsync(userId, model);
            if (!ok)
            {
                TempData["ErrorMessage"] = "Could not update your profile. Please try again.";
                return View(model);
            }

            TempData["SuccessMessage"] = "Your profile has been updated.";
            return RedirectToAction(nameof(Profile));
        }

        // GET /Users/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

        // POST /Users/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Forbid();

            if (!ModelState.IsValid) return View(model);

            var (ok, error) = await _userService.ChangePasswordAsync(userId, model);
            if (!ok)
            {
                _logger.LogInformation("Failed password change attempt for UserId={UserId}: {Error}", userId, error);
                ModelState.AddModelError(string.Empty, error ?? "Could not change password.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Your password has been changed.";
            return RedirectToAction(nameof(Profile));
        }

        // GET /Users/DeleteAccount  (confirmation page)
        [HttpGet]
        public async Task<IActionResult> DeleteAccount()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Forbid();

            var activeCount = await _userService.CountActiveBookingsAsync(userId);
            return View(new DeleteAccountViewModel { ActiveBookingCount = activeCount });
        }

        // POST /Users/DeleteAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(DeleteAccountViewModel model)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Forbid();

            if (!ModelState.IsValid)
            {
                model.ActiveBookingCount = await _userService.CountActiveBookingsAsync(userId);
                return View(model);
            }

            var (ok, error, activeCount) = await _userService.DeleteAccountAsync(userId, model.Password);
            if (!ok)
            {
                _logger.LogInformation("Account deletion blocked for UserId={UserId}: {Error}", userId, error);
                TempData["ErrorMessage"] = error;
                return RedirectToAction(nameof(Profile));
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "Your account has been deleted. We're sorry to see you go.";
            _logger.LogWarning("UserId={UserId} self-deleted their account.", userId);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // GET /Users/MyBookings
        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Forbid();

            var vm = await _userService.GetBookingsAsync(userId);
            return View(vm);
        }
    }
}
