using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VrindavanProfile.Web.Data;
using VrindavanProfile.Web.Data.DbEntities;
using VrindavanProfile.Web.Services;

namespace VrindavanProfile.Web.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _db;
    
    public AuthController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Admin");
        return View();
    }
    
    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !PasswordHasher.Verify(password, user.PasswordHash))
        {
            ModelState.AddModelError("", "अमान्य युजरनेम वा पासवर्ड।");
            return View();
        }

        // Block inactive accounts
        if (!user.IsActive)
        {
            ModelState.AddModelError("", "तपाईंको खाता निष्क्रिय गरिएको छ। कृपया मुख्य प्रशासकलाई सम्पर्क गर्नुहोस्।");
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("UserId", user.Id.ToString()),
            new Claim("WardNo", user.WardNo?.ToString() ?? "") // "" means global admin
        };
        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Admin");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Register(string username, string email,
        string password, string confirmPassword)
    {
        if (password != confirmPassword)
        {
            ModelState.AddModelError("", "पासवर्ड र पुष्टि पासवर्ड मिलेन।");
            return View();
        }

        var exists = await _db.AppUsers.AnyAsync(u => u.Username == username);
        if (exists)
        {
            ModelState.AddModelError("", "युजरनेम पहिले नै अवस्थित छ।");
            return View();
        }

        var user = new AppUser
        {
            Username = username,
            Email = email,
            PasswordHash = PasswordHasher.Hash(password),
            Role = "admin",
            CreatedAt = DateTime.UtcNow
        };

        _db.AppUsers.Add(user);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"नयाँ प्रशासक '{username}' सफलतापूर्वक सिर्जना गरियो।";
        return RedirectToAction("Index", "Admin");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Auth");
    }
}