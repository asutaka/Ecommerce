using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminAuth")]
public class DataMigrationController(EcommerceDbContext dbContext) : Controller
{
    /// <summary>
    /// Generate slugs for all existing products that don't have one
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> GenerateProductSlugs()
    {
        try
        {
            // Find all products with empty or null slugs
            var productsWithoutSlug = await dbContext.Products
                .Where(p => string.IsNullOrEmpty(p.Slug))
                .ToListAsync();

            if (!productsWithoutSlug.Any())
            {
                TempData["Success"] = "Tất cả sản phẩm đã có slug!";
                return RedirectToAction("Index", "Products");
            }

            int updated = 0;
            foreach (var product in productsWithoutSlug)
            {
                product.Slug = SlugHelper.GenerateProductSlug(product.Name, product.Id);
                product.UpdatedAt = DateTime.UtcNow;
                updated++;
            }

            await dbContext.SaveChangesAsync();

            TempData["Success"] = $"Đã generate slug cho {updated} sản phẩm thành công!";
            return RedirectToAction("Index", "Products");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi generate slugs: {ex.Message}";
            return RedirectToAction("Index", "Products");
        }
    }

    /// <summary>
    /// Regenerate ALL product slugs (use with caution - will break existing URLs)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RegenerateAllProductSlugs()
    {
        try
        {
            var allProducts = await dbContext.Products.ToListAsync();

            foreach (var product in allProducts)
            {
                product.Slug = SlugHelper.GenerateProductSlug(product.Name, product.Id);
                product.UpdatedAt = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync();

            TempData["Success"] = $"Đã regenerate slug cho {allProducts.Count} sản phẩm!";
            return RedirectToAction("Index", "Products");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi regenerate slugs: {ex.Message}";
            return RedirectToAction("Index", "Products");
        }
    }
}
