using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class CategoriesController(EcommerceDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? searchTerm, int? pageNumber)
    {
        var query = dbContext.Categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(term) || 
                                     (x.Description != null && x.Description.ToLower().Contains(term)));
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        int pageSize = 10;
        var categories = await PaginatedList<Category>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize);

        ViewBag.SearchTerm = searchTerm;
        return View(categories);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CategoryFormViewModel
        {
            Categories = await GetCategoriesForDropdown()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategoriesForDropdown();
            return View(model);
        }

        var category = new Category
        {
            Name = model.Name,
            Description = model.Description,
            ParentId = model.ParentId
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Thêm danh mục thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var category = await dbContext.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        var model = new CategoryFormViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId,
            Categories = await GetCategoriesForDropdown(id)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, CategoryFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategoriesForDropdown(id);
            return View(model);
        }

        var category = await dbContext.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        // Prevent setting parent to itself or creating circular reference
        if (model.ParentId == id)
        {
            ModelState.AddModelError("ParentId", "Danh mục không thể là cha của chính nó");
            model.Categories = await GetCategoriesForDropdown(id);
            return View(model);
        }

        category.Name = model.Name;
        category.Description = model.Description;
        category.ParentId = model.ParentId;
        category.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Cập nhật danh mục thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await dbContext.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        // Check if category is used by any product
        var hasProducts = await dbContext.ProductCategories.AnyAsync(x => x.CategoryId == id);
        if (hasProducts)
        {
            TempData["Error"] = "Không thể xóa danh mục đang có sản phẩm";
            return RedirectToAction(nameof(Index));
        }

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Xóa danh mục thành công";
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetCategoriesForDropdown(Guid? excludeId = null)
    {
        var categories = await dbContext.Categories
            .Where(c => excludeId == null || c.Id != excludeId)
            .OrderBy(c => c.Name)
            .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToListAsync();
        
        return categories;
    }
}
