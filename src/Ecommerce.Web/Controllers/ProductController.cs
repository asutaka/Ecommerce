using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Controllers;

public class ProductController(EcommerceDbContext dbContext) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var product = await dbContext.Products
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        // Get related products from same category
        var relatedProducts = await dbContext.Products
            .Where(x => x.CategoryId == product.CategoryId && x.Id != product.Id)
            .OrderByDescending(x => x.IsFeatured)
            .ThenByDescending(x => x.CreatedAt)
            .Take(4)
            .Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Images = p.Images,
                Price = p.Price,
                IsFeatured = p.IsFeatured
            })
            .ToListAsync();

        var model = new ProductDetailViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Images = product.Images,
            Price = product.Price,
            IsFeatured = product.IsFeatured,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? "N/A",
            RelatedProducts = relatedProducts
        };

        return View(model);
    }
}
