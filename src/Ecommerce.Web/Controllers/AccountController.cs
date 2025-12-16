using Ecommerce.Web.Services;
using Ecommerce.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Infrastructure.Persistence;

namespace Ecommerce.Web.Controllers;

[AllowAnonymous]
public class AccountController(
    ICustomerAuthService customerAuthService,
    ICartService cartService,
    EcommerceDbContext dbContext,
    ILogger<AccountController> logger) : Controller
{
    private const string CustomerAuthScheme = "CustomerAuth";

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(CustomerLoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var customer = await customerAuthService.ValidateCredentialsAsync(model.Email, model.Password);

        if (customer == null)
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        // Check if password is default (12345)
        if (BCrypt.Net.BCrypt.Verify("12345", customer.PasswordHash))
        {
            // Store customer ID in TempData for password change
            TempData["RequirePasswordChange"] = customer.Id.ToString();
            TempData["Info"] = "Bạn đang sử dụng mật khẩu mặc định. Vui lòng đổi mật khẩu mới để tiếp tục.";
            return RedirectToAction(nameof(ChangePassword));
        }

        // Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.Role, "Customer")
        };

        if (!string.IsNullOrEmpty(customer.FullName))
        {
            claims.Add(new Claim(ClaimTypes.Name, customer.FullName));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CustomerAuthScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(12)
        };

        await HttpContext.SignInAsync(
            CustomerAuthScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        await customerAuthService.UpdateLastLoginAsync(customer.Id);
        
        // Merge cart
        var sessionId = HttpContext.Session.GetString("_CartId");
        if (!string.IsNullOrEmpty(sessionId))
        {
            await cartService.MergeCartAsync(sessionId, customer.Id);
        }

        logger.LogInformation("Customer {Email} logged in", customer.Email);

        return RedirectToLocal(returnUrl);
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(CustomerRegisterViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var customer = await customerAuthService.RegisterAsync(
            model.Email,
            model.Password,
            model.FullName,
            model.Phone);

        if (customer == null)
        {
            ModelState.AddModelError(string.Empty, "Email này đã được đăng ký.");
            return View(model);
        }

        // Auto login after registration
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.Role, "Customer")
        };

        if (!string.IsNullOrEmpty(customer.FullName))
        {
            claims.Add(new Claim(ClaimTypes.Name, customer.FullName));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CustomerAuthScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
        };

        await HttpContext.SignInAsync(
            CustomerAuthScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // Merge cart
        var sessionId = HttpContext.Session.GetString("_CartId");
        if (!string.IsNullOrEmpty(sessionId))
        {
            await cartService.MergeCartAsync(sessionId, customer.Id);
        }

        logger.LogInformation("New customer registered: {Email}", customer.Email);

        TempData["Success"] = "Đăng ký thành công! Chào mừng bạn đến với Moderno.";
        return RedirectToLocal(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CustomerAuthScheme);
        logger.LogInformation("Customer logged out");
        return RedirectToAction("Index", "Home");
    }

    [Authorize(AuthenticationSchemes = CustomerAuthScheme)]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (customerIdClaim == null || !Guid.TryParse(customerIdClaim, out var customerId))
        {
            return RedirectToAction(nameof(Login));
        }

        var customer = await customerAuthService.GetByIdAsync(customerId);
        if (customer == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var recentOrders = await dbContext.Orders
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(5)
            .Select(x => new OrderSummaryViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Total = x.Total,
                Status = x.Status.ToString()
            })
            .ToListAsync();

        var model = new CustomerProfileViewModel
        {
            Id = customer.Id,
            Email = customer.Email,
            FullName = customer.FullName,
            Phone = customer.Phone,
            LastLoginAt = customer.LastLoginAt,
            ShippingAddress1 = customer.ShippingAddress1,
            ShippingAddress2 = customer.ShippingAddress2,
            ShippingAddress3 = customer.ShippingAddress3,
            TotalOrders = await dbContext.Orders.CountAsync(x => x.CustomerId == customerId),
            RecentOrders = recentOrders
        };

        return View(model);
    }

    [Authorize(AuthenticationSchemes = CustomerAuthScheme)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Thông tin không hợp lệ";
            return RedirectToAction(nameof(Profile));
        }

        var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (customerIdClaim == null || !Guid.TryParse(customerIdClaim, out var customerId))
        {
            return RedirectToAction(nameof(Login));
        }

        var success = await customerAuthService.UpdateProfileAsync(customerId, model);
        if (success)
        {
            TempData["Success"] = "Cập nhật thông tin thành công";
        }
        else
        {
            TempData["Error"] = "Không thể cập nhật thông tin";
        }

        return RedirectToAction(nameof(Profile));
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, provider);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, $"Lỗi từ nhà cung cấp: {remoteError}");
            return RedirectToAction(nameof(Login));
        }

        var info = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (info?.Principal == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var nameIdentifier = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var name = info.Principal.FindFirstValue(ClaimTypes.Name);
        var provider = info.Properties?.Items[".AuthScheme"] ?? "Unknown";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nameIdentifier))
        {
            ModelState.AddModelError(string.Empty, "Không thể lấy thông tin từ nhà cung cấp.");
            return RedirectToAction(nameof(Login));
        }

        var customer = await customerAuthService.FindOrCreateExternalLoginAsync(
            provider,
            nameIdentifier,
            email,
            name);

        if (customer == null)
        {
            ModelState.AddModelError(string.Empty, "Không thể tạo tài khoản.");
            return RedirectToAction(nameof(Login));
        }

        // Sign in with customer auth scheme
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.Role, "Customer")
        };

        if (!string.IsNullOrEmpty(customer.FullName))
        {
            claims.Add(new Claim(ClaimTypes.Name, customer.FullName));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CustomerAuthScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
        };

        await HttpContext.SignInAsync(
            CustomerAuthScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        await customerAuthService.UpdateLastLoginAsync(customer.Id);
        
        // Merge cart
        var sessionId = HttpContext.Session.GetString("_CartId");
        if (!string.IsNullOrEmpty(sessionId))
        {
            await cartService.MergeCartAsync(sessionId, customer.Id);
        }

        logger.LogInformation("Customer {Email} logged in via {Provider}", customer.Email, provider);

        return RedirectToLocal(returnUrl);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ChangePassword()
    {
        // Check if this is a required password change
        if (TempData["RequirePasswordChange"] == null)
        {
            return RedirectToAction(nameof(Login));
        }

        // Pass customer ID via ViewBag to persist it in the view
        ViewBag.CustomerId = TempData["RequirePasswordChange"]?.ToString();
        ViewBag.RequirePasswordChange = true;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model, string customerId)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CustomerId = customerId;
            ViewBag.RequirePasswordChange = true;
            return View(model);
        }

        // Check if new password is not default
        if (model.NewPassword == "12345")
        {
            ModelState.AddModelError(nameof(model.NewPassword), "Mật khẩu mới không được là mật khẩu mặc định (12345).");
            ViewBag.CustomerId = customerId;
            ViewBag.RequirePasswordChange = true;
            return View(model);
        }

        // Get customer ID from form parameter
        if (string.IsNullOrEmpty(customerId) || !Guid.TryParse(customerId, out var customerGuid))
        {
            return RedirectToAction(nameof(Login));
        }

        var customer = await customerAuthService.GetByIdAsync(customerGuid);
        if (customer == null)
        {
            return RedirectToAction(nameof(Login));
        }

        // Update password
        customer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        customer.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        // Auto login after password change
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.Role, "Customer")
        };

        if (!string.IsNullOrEmpty(customer.FullName))
        {
            claims.Add(new Claim(ClaimTypes.Name, customer.FullName));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CustomerAuthScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
        };

        await HttpContext.SignInAsync(
            CustomerAuthScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        await customerAuthService.UpdateLastLoginAsync(customer.Id);

        // Merge cart
        var sessionId = HttpContext.Session.GetString("_CartId");
        if (!string.IsNullOrEmpty(sessionId))
        {
            await cartService.MergeCartAsync(sessionId, customer.Id);
        }

        logger.LogInformation("Customer {Email} changed password and logged in", customer.Email);

        TempData["Success"] = "Đổi mật khẩu thành công!";
        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }
}
