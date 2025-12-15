using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminAuth")]
public class AdminUsersController(EcommerceDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? searchTerm, int? pageNumber)
    {
        var query = dbContext.AdminUsers.Include(x => x.Group).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(x => x.Username.ToLower().Contains(term) || 
                                     x.Email.ToLower().Contains(term) ||
                                     (x.FullName != null && x.FullName.ToLower().Contains(term)));
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        int pageSize = 10;
        var adminUsers = await PaginatedList<AdminUser>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize);

        ViewBag.SearchTerm = searchTerm;
        return View(adminUsers);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new AdminUserFormViewModel
        {
            Groups = await GetGroupsSelectList()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminUserFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Groups = await GetGroupsSelectList();
            return View(model);
        }

        // Check if username already exists
        if (await dbContext.AdminUsers.AnyAsync(x => x.Username == model.Username))
        {
            ModelState.AddModelError("Username", "Username đã tồn tại");
            model.Groups = await GetGroupsSelectList();
            return View(model);
        }

        // Check if email already exists
        if (await dbContext.AdminUsers.AnyAsync(x => x.Email == model.Email))
        {
            ModelState.AddModelError("Email", "Email đã tồn tại");
            model.Groups = await GetGroupsSelectList();
            return View(model);
        }

        // Validate password for create
        if (string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError("Password", "Mật khẩu là bắt buộc");
            model.Groups = await GetGroupsSelectList();
            return View(model);
        }

        var adminUser = new AdminUser
        {
            Username = model.Username,
            Email = model.Email,
            FullName = model.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            GroupId = model.GroupId,
            IsActive = model.IsActive
        };

        dbContext.AdminUsers.Add(adminUser);
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Thêm admin user thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var adminUser = await dbContext.AdminUsers.FindAsync(id);
        if (adminUser == null)
        {
            return NotFound();
        }

        var model = new AdminUserFormViewModel
        {
            Id = adminUser.Id,
            Username = adminUser.Username,
            Email = adminUser.Email,
            FullName = adminUser.FullName,
            GroupId = adminUser.GroupId,
            IsActive = adminUser.IsActive,
            Groups = await GetGroupsSelectList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AdminUserFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.Groups = await GetGroupsSelectList();
            return View(model);
        }

        var adminUser = await dbContext.AdminUsers.FindAsync(id);
        if (adminUser == null)
        {
            return NotFound();
        }

        // Check if email already exists (excluding current user)
        if (await dbContext.AdminUsers.AnyAsync(x => x.Email == model.Email && x.Id != id))
        {
            ModelState.AddModelError("Email", "Email đã tồn tại");
            model.Groups = await GetGroupsSelectList();
            return View(model);
        }

        adminUser.Email = model.Email;
        adminUser.FullName = model.FullName;
        adminUser.GroupId = model.GroupId;
        adminUser.IsActive = model.IsActive;
        adminUser.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Cập nhật admin user thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var adminUser = await dbContext.AdminUsers.FindAsync(id);
        if (adminUser == null)
        {
            return NotFound();
        }

        dbContext.AdminUsers.Remove(adminUser);
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Xóa admin user thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var adminUser = await dbContext.AdminUsers.FindAsync(id);
        if (adminUser == null)
        {
            return NotFound();
        }

        adminUser.IsActive = !adminUser.IsActive;
        adminUser.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        TempData["Success"] = $"Đã {(adminUser.IsActive ? "kích hoạt" : "vô hiệu hóa")} admin user";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(Guid id)
    {
        var adminUser = await dbContext.AdminUsers.FindAsync(id);
        if (adminUser == null)
        {
            return NotFound();
        }

        var model = new ResetPasswordViewModel
        {
            AdminUserId = adminUser.Id,
            Username = adminUser.Username,
            Email = adminUser.Email
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var adminUser = await dbContext.AdminUsers.FindAsync(model.AdminUserId);
        if (adminUser == null)
        {
            return NotFound();
        }

        adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        adminUser.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Reset mật khẩu thành công";
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> GetGroupsSelectList()
    {
        var groups = await dbContext.Groups
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name} ({x.Code})"
            })
            .ToListAsync();

        groups.Insert(0, new SelectListItem { Value = "", Text = "-- Không thuộc nhóm nào --" });
        return groups;
    }
}
