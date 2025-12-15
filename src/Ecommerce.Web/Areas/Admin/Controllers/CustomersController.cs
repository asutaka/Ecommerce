using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class CustomersController(EcommerceDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? searchTerm, int? pageNumber, bool? emailConfirmed)
    {
        var query = dbContext.Customers
            .Include(c => c.Orders)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(x => 
                (x.Email != null && x.Email.ToLower().Contains(term)) || 
                (x.FullName != null && x.FullName.ToLower().Contains(term)) ||
                (x.Phone != null && x.Phone.Contains(term)));
        }

        if (emailConfirmed.HasValue)
        {
            query = query.Where(x => x.EmailConfirmed == emailConfirmed.Value);
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        int pageSize = 15;
        var customers = await PaginatedList<Customer>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize);

        ViewBag.SearchTerm = searchTerm;
        ViewBag.EmailConfirmed = emailConfirmed;
        return View(customers);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var customer = await dbContext.Customers
            .Include(c => c.Orders)
                .ThenInclude(o => o.Items)
            .Include(c => c.ExternalLogins)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> SendEmail(Guid id, string subject, string message)
    {
        var customer = await dbContext.Customers.FindAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        // TODO: Implement actual email sending logic here
        // For now, just return success message
        // await _emailService.SendEmailAsync(customer.Email, subject, message);

        TempData["Success"] = $"Email đã được gửi đến {customer.Email}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(Guid id)
    {
        var customer = await dbContext.Customers.FindAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        // Hash password "12345" 
        // Using BCrypt for password hashing
        var defaultPassword = "12345";
        customer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword);
        customer.UpdatedAt = DateTime.UtcNow;
        
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Đã cập nhật thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await dbContext.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null)
        {
            return NotFound();
        }

        // Check if customer has orders
        if (customer.Orders.Any())
        {
            TempData["Error"] = "Không thể xóa khách hàng đã có đơn hàng. Hãy xóa đơn hàng trước.";
            return RedirectToAction(nameof(Index));
        }

        dbContext.Customers.Remove(customer);
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Xóa tài khoản khách hàng thành công";
        return RedirectToAction(nameof(Index));
    }
}
