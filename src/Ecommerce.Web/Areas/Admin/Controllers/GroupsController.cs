using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Ecommerce.Web.Helpers;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminAuth")]
public class GroupsController(EcommerceDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? searchTerm, int? pageNumber)
    {
        var query = dbContext.Groups.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(x => x.Code.ToLower().Contains(term) || 
                                     x.Name.ToLower().Contains(term) ||
                                     (x.Description != null && x.Description.ToLower().Contains(term)));
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        int pageSize = 10;
        var groups = await PaginatedList<Group>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize);

        ViewBag.SearchTerm = searchTerm;
        return View(groups);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new GroupFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GroupFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Check if code already exists
        if (await dbContext.Groups.AnyAsync(x => x.Code == model.Code))
        {
            ModelState.AddModelError("Code", "Mã nhóm đã tồn tại");
            return View(model);
        }

        var group = new Group
        {
            Code = model.Code,
            Name = model.Name,
            Description = model.Description,
            IsActive = model.IsActive
        };

        dbContext.Groups.Add(group);
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Thêm nhóm thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var group = await dbContext.Groups.FindAsync(id);
        if (group == null)
        {
            return NotFound();
        }

        var model = new GroupFormViewModel
        {
            Id = group.Id,
            Code = group.Code,
            Name = group.Name,
            Description = group.Description,
            IsActive = group.IsActive
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, GroupFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var group = await dbContext.Groups.FindAsync(id);
        if (group == null)
        {
            return NotFound();
        }

        // Check if code already exists (excluding current group)
        if (await dbContext.Groups.AnyAsync(x => x.Code == model.Code && x.Id != id))
        {
            ModelState.AddModelError("Code", "Mã nhóm đã tồn tại");
            return View(model);
        }

        group.Code = model.Code;
        group.Name = model.Name;
        group.Description = model.Description;
        group.IsActive = model.IsActive;
        group.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Cập nhật nhóm thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var group = await dbContext.Groups.FindAsync(id);
        if (group == null)
        {
            return NotFound();
        }

        dbContext.Groups.Remove(group);
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Xóa nhóm thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var group = await dbContext.Groups.FindAsync(id);
        if (group == null)
        {
            return NotFound();
        }

        group.IsActive = !group.IsActive;
        group.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        TempData["Success"] = $"Đã {(group.IsActive ? "kích hoạt" : "vô hiệu hóa")} nhóm";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Permissions(Guid id)
    {
        var group = await dbContext.Groups.FindAsync(id);
        if (group == null)
        {
            return NotFound();
        }

        var model = new PermissionsViewModel
        {
            GroupId = group.Id,
            GroupCode = group.Code,
            GroupName = group.Name,
            Permissions = new Dictionary<string, int>(group.Permissions)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Permissions(PermissionsViewModel model)
    {
        var group = await dbContext.Groups.FindAsync(model.GroupId);
        if (group == null)
        {
            return NotFound();
        }

        // Create NEW dictionary instead of modifying existing one
        // This is required for EF Core change tracking with HasConversion
        var newPermissions = new Dictionary<string, int>();
        
        // Process all modules and their permissions
        foreach (var module in PermissionsViewModel.AllModules)
        {
            int permissions = 0;
            
            // Check each permission type from form
            var viewKey = $"view_{module.Key}";
            var addKey = $"add_{module.Key}";
            var editKey = $"edit_{module.Key}";
            var deleteKey = $"delete_{module.Key}";
            
            if (Request.Form.ContainsKey(viewKey) && Request.Form[viewKey] == "true")
                permissions |= (int)EPermission.View;
            
            if (Request.Form.ContainsKey(addKey) && Request.Form[addKey] == "true")
                permissions |= (int)EPermission.Add;
            
            if (Request.Form.ContainsKey(editKey) && Request.Form[editKey] == "true")
                permissions |= (int)EPermission.Edit;
            
            if (Request.Form.ContainsKey(deleteKey) && Request.Form[deleteKey] == "true")
                permissions |= (int)EPermission.Delete;
            
            // Only add to dictionary if has any permissions
            if (permissions > 0)
            {
                newPermissions[module.Key] = permissions;
            }
        }

        // Assign new dictionary to trigger EF Core change detection
        group.Permissions = newPermissions;

        group.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Cập nhật phân quyền thành công";
        return RedirectToAction(nameof(Index));
    }
}
