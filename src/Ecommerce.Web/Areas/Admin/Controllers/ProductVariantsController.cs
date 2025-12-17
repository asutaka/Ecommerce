using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminAuth")]
public class ProductVariantsController(EcommerceDbContext dbContext) : Controller
{
    // Get all variants for a product (used by Edit page)
    [HttpGet]
    public async Task<IActionResult> GetVariants(Guid productId)
    {
        var variants = await dbContext.ProductVariants
            .Where(v => v.ProductId == productId)
            .OrderBy(v => v.Color)
            .ThenBy(v => v.Size)
            .Select(v => new ProductVariantListViewModel
            {
                Id = v.Id,
                SKU = v.SKU,
                Color = v.Color,
                Size = v.Size,
                Stock = v.Stock,
                Price = v.Price,
                OriginalPrice = v.OriginalPrice,
                ImageUrls = v.ImageUrls,
                IsActive = v.IsActive
            })
            .ToListAsync();

        return Json(variants);
    }

    // Create variant
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductVariantFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if product exists
        var productExists = await dbContext.Products.AnyAsync(p => p.Id == model.ProductId);
        if (!productExists)
        {
            return NotFound(new { message = "Sản phẩm không tồn tại" });
        }

        // Check if SKU already exists
        var skuExists = await dbContext.ProductVariants.AnyAsync(v => v.SKU == model.SKU);
        if (skuExists)
        {
            return BadRequest(new { message = "SKU đã tồn tại" });
        }

        var variant = new ProductVariant
        {
            ProductId = model.ProductId,
            SKU = model.SKU,
            Color = model.Color,
            Size = model.Size,
            Stock = model.Stock,
            Price = model.Price,
            OriginalPrice = model.OriginalPrice,
            ImageUrls = model.ImageUrls,
            IsActive = model.IsActive
        };

        dbContext.ProductVariants.Add(variant);
        await dbContext.SaveChangesAsync();

        var result = new ProductVariantListViewModel
        {
            Id = variant.Id,
            SKU = variant.SKU,
            Color = variant.Color,
            Size = variant.Size,
            Stock = variant.Stock,
            Price = variant.Price,
            OriginalPrice = variant.OriginalPrice,
            ImageUrls = variant.ImageUrls,
            IsActive = variant.IsActive
        };

        return Ok(result);
    }

    // Update variant
    [HttpPut]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductVariantFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var variant = await dbContext.ProductVariants.FindAsync(id);
        if (variant == null)
        {
            return NotFound(new { message = "Variant không tồn tại" });
        }

        // Check if SKU is being changed and if new SKU already exists
        if (variant.SKU != model.SKU)
        {
            var skuExists = await dbContext.ProductVariants.AnyAsync(v => v.SKU == model.SKU && v.Id != id);
            if (skuExists)
            {
                return BadRequest(new { message = "SKU đã tồn tại" });
            }
        }

        variant.SKU = model.SKU;
        variant.Color = model.Color;
        variant.Size = model.Size;
        variant.Stock = model.Stock;
        variant.Price = model.Price;
        variant.OriginalPrice = model.OriginalPrice;
        variant.ImageUrls = model.ImageUrls;
        variant.IsActive = model.IsActive;
        variant.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        var result = new ProductVariantListViewModel
        {
            Id = variant.Id,
            SKU = variant.SKU,
            Color = variant.Color,
            Size = variant.Size,
            Stock = variant.Stock,
            Price = variant.Price,
            OriginalPrice = variant.OriginalPrice,
            ImageUrls = variant.ImageUrls,
            IsActive = variant.IsActive
        };

        return Ok(result);
    }

    // Delete variant
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        var variant = await dbContext.ProductVariants.FindAsync(id);
        if (variant == null)
        {
            return NotFound(new { message = "Variant không tồn tại" });
        }

        dbContext.ProductVariants.Remove(variant);
        await dbContext.SaveChangesAsync();

        return Ok(new { message = "Đã xóa variant thành công" });
    }

    // Toggle Active status
    [HttpPost]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var variant = await dbContext.ProductVariants.FindAsync(id);
        if (variant == null)
        {
            return NotFound(new { message = "Variant không tồn tại" });
        }

        variant.IsActive = !variant.IsActive;
        variant.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        return Ok(new { isActive = variant.IsActive });
    }
}
