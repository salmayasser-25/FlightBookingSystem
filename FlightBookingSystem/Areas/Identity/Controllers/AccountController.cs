using FlightBookingSystem.Models;
///using FlightBookingSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FlightBookingSystem.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailService _emailService;

        public AccountController(ApplicationDbContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private string GenerateToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        private string GenerateOtp() => RandomNumberGenerator.GetInt32(100000, 999999).ToString();


        // ================================================================
        // 1. REGISTER
        // ================================================================
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var exists = await _db.Users.AnyAsync(u => u.Email == model.Email);
            if (exists)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            var now = DateTime.Now;
            var token = GenerateToken();

            var user = new User
            {
                FName = model.FName,
                LName = model.LName,
                Email = model.Email,
                Password = HashPassword(model.Password),
                CreatedDay = now.Day,
                CreatedMonth = now.Month,
                CreatedYear = now.Year,
                EmailConfirmed = false,
                EmailConfirmToken = token,
                EmailConfirmTokenExpiry = DateTime.Now.AddHours(24),
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var confirmLink = Url.Action("ConfirmEmail", "Account",
                new { area = "Identity", token, email = user.Email },
                Request.Scheme)!;

            await _emailService.SendConfirmationEmailAsync(user.Email,
                $"{user.FName} {user.LName}", confirmLink);

            TempData["SuccessMessage"] = "Account created! Please check your email to confirm your account.";
            return RedirectToAction("Login");
        }


        // ================================================================
        // 2. CONFIRM EMAIL
        // ================================================================
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var model = new ConfirmEmailViewModel { Email = email };
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || user.EmailConfirmToken != token ||
                user.EmailConfirmTokenExpiry < DateTime.Now)
            {
                model.IsSuccess = false;
                model.Message = "Invalid or expired confirmation link.";
                return View(model);
            }

            if (user.EmailConfirmed)
            {
                model.IsSuccess = true;
                model.Message = "Your email is already confirmed.";
                return View(model);
            }

            user.EmailConfirmed = true;
            user.EmailConfirmToken = null;
            user.EmailConfirmTokenExpiry = null;
            await _db.SaveChangesAsync();

            var otp = GenerateOtp();
            user.OtpCode = otp;
            user.OtpExpiry = DateTime.Now.AddMinutes(10);
            user.OtpVerified = false;
            await _db.SaveChangesAsync();

            await _emailService.SendOtpEmailAsync(user.Email,
                $"{user.FName} {user.LName}", otp);

            TempData["OtpEmail"] = email;
            model.IsSuccess = true;
            model.Message = "Email confirmed! We sent an OTP code to complete your registration.";

            return RedirectToAction("ValidateOtp", new { email });
        }


        // ================================================================
        // 3. VALIDATE OTP
        // ================================================================
        [HttpGet]
        public IActionResult ValidateOtp(string email)
        {
            return View(new OtpViewModel { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateOtp(OtpViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || user.OtpCode != model.OtpCode || user.OtpExpiry < DateTime.Now)
            {
                ModelState.AddModelError("OtpCode", "Invalid or expired OTP code.");
                return View(model);
            }

            user.OtpVerified = true;
            user.OtpCode = null;
            user.OtpExpiry = null;
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Verification successful! You can now log in.";
            return RedirectToAction("Login");
        }


        // ================================================================
        // 4. LOGIN
        // ================================================================
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || user.Password != HashPassword(model.Password))
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Please confirm your email address first.");
                return View(model);
            }

            if (!user.OtpVerified)
            {
                ModelState.AddModelError("", "Please complete OTP verification first.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email,           user.Email),
                new Claim(ClaimTypes.GivenName,       user.FName),
                new Claim(ClaimTypes.Surname,         user.LName),
                new Claim("FullName",                 $"{user.FName} {user.LName}"),
                new Claim(ClaimTypes.Role, user.Admin != null ? "Admin" : "User"),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(30)
                    : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProps);

            return RedirectToAction("Index", "Home", new { area = "" });
        }


        // ================================================================
        // 5. FORGOT PASSWORD
        // ================================================================
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            TempData["SuccessMessage"] = "If this email is registered, you will receive a password reset link.";

            if (user != null && user.EmailConfirmed)
            {
                var token = GenerateToken();
                user.ResetPasswordToken = token;
                user.ResetPasswordTokenExpiry = DateTime.Now.AddHours(1);
                await _db.SaveChangesAsync();

                var resetLink = Url.Action("ResetPassword", "Account",
                    new { area = "Identity", token, email = user.Email },
                    Request.Scheme)!;

                await _emailService.SendResetPasswordEmailAsync(user.Email,
                    $"{user.FName} {user.LName}", resetLink);
            }

            return RedirectToAction("Login");
        }


        // ================================================================
        // 6. RESET PASSWORD
        // ================================================================
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || user.ResetPasswordToken != model.Token ||
                user.ResetPasswordTokenExpiry < DateTime.Now)
            {
                ModelState.AddModelError("", "Invalid or expired reset link.");
                return View(model);
            }

            user.Password = HashPassword(model.NewPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiry = null;
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Password changed successfully! You can now log in.";
            return RedirectToAction("Login");
        }


        // ================================================================
        // 7. LOGOUT
        // ================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }


        // ================================================================
        // HELPER: Resend OTP
        // ================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendOtp(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && user.EmailConfirmed)
            {
                var otp = GenerateOtp();
                user.OtpCode = otp;
                user.OtpExpiry = DateTime.Now.AddMinutes(10);
                await _db.SaveChangesAsync();

                await _emailService.SendOtpEmailAsync(user.Email,
                    $"{user.FName} {user.LName}", otp);
            }

            TempData["InfoMessage"] = "OTP code resent to your email.";
            return RedirectToAction("ValidateOtp", new { email });
        }
    }
}
