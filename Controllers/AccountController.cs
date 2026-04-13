using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserAdminPanel.Data;
using UserAdminPanel.Models;

namespace UserAdminPanel.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (_db.Users.Any(u => u.Email == model.Email))
            {
                TempData["ErrorMessage"] = "This email is already registered. Please login or use a different email.";
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password,
                Status = UserStatus.Unverified,
                RegistrationTime = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

#pragma warning disable CS4014
            Task.Run(() => SendEmailAsync(user.Email));
#pragma warning restore CS4014

            TempData["SuccessMessage"] = "Registration successful! You can now log in.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user != null && user.Status == UserStatus.Unverified)
            {
                user.Status = UserStatus.Active;
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Email verified successfully! You can now log in.";
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _db.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid email or password.";
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            if (user.Status == UserStatus.Blocked)
            {
                TempData["ErrorMessage"] = "Your account has been blocked. Please contact support.";
                ModelState.AddModelError(string.Empty, "Your account is blocked.");
                return View(model);
            }

            user.LastLoginTime = DateTime.UtcNow;
            user.LoginTimestamps ??= new List<DateTime>();
            user.LoginTimestamps.Add(DateTime.UtcNow);
            await _db.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private async Task SendEmailAsync(string email)
        {
            await Task.Delay(2000);
            Console.WriteLine($"[ASYNC] Email sent to {email}");
        }
    }
}
