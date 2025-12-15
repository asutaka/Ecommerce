using Ecommerce.Web.Areas.Admin.ViewModels;
using Ecommerce.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
public class AccountController(
    IAdminAuthService adminAuthService,
    ILogger<AccountController> logger) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var admin = await adminAuthService.ValidateCredentialsAsync(model.EmailOrUsername, model.Password);

        if (admin == null)
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(model);
        }

        // Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new Claim(ClaimTypes.Name, admin.Username),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim(ClaimTypes.Role, "Admin")
        };

        if (!string.IsNullOrEmpty(admin.FullName))
        {
            claims.Add(new Claim(ClaimTypes.GivenName, admin.FullName));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(12)
        };

        await HttpContext.SignInAsync(
            "AdminAuth",
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        logger.LogInformation("Admin {Username} logged in", admin.Username);

        return RedirectToLocal(returnUrl);
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = "AdminAuth")]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var admin = await adminAuthService.GetAdminByIdAsync(Guid.Parse(userId));
        if (admin == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var model = new ProfileViewModel
        {
            Username = admin.Username,
            Email = admin.Email,
            FullName = admin.FullName
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(AuthenticationSchemes = "AdminAuth")]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var success = await adminAuthService.UpdateProfileAsync(
            Guid.Parse(userId),
            model.Email,
            model.FullName,
            model.CurrentPassword,
            model.NewPassword
        );

        if (!success)
        {
            ModelState.AddModelError(string.Empty, "Không thể cập nhật hồ sơ. Vui lòng kiểm tra mật khẩu hiện tại.");
            return View(model);
        }

        TempData["Success"] = "Cập nhật hồ sơ thành công";
        return RedirectToAction(nameof(Profile));
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("AdminAuth");
        
        // Explicitly delete the admin auth cookie
        Response.Cookies.Delete("Ecommerce.Admin.Auth");
        
        // Add cache control headers to prevent caching
        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";
        
        logger.LogInformation("Admin logged out");
        return RedirectToAction(nameof(Login));
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }
}
