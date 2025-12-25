using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Models;
using Ecommerce.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Controllers;

public class CategoryController(EcommerceDbContext dbContext) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(Guid id, int? pageNumber, string? search, decimal? minPrice, decimal? maxPrice)
    {
        var category = await dbContext.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        var query = dbContext.Products
            .Where(x => x.IsActive && x.ProductCategories.Any(pc => pc.CategoryId == id));

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Name.Contains(search));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(x => x.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(x => x.Price <= maxPrice.Value);
        }

        query = query.OrderByDescending(x => x.IsFeatured)
            .ThenByDescending(x => x.CreatedAt);

        int pageSize = 12;
        var products = await PaginatedList<Infrastructure.Entities.Product>.CreateAsync(
            query.AsNoTracking(), 
            pageNumber ?? 1, 
            pageSize);

        var model = new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ProductCount = await dbContext.ProductCategories.CountAsync(pc => pc.CategoryId == id),
            Products = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Slug = p.Slug,
                Name = p.Name,
                Description = p.Description,
                Images = p.Images,
                Price = p.Price,
                OriginalPrice = p.OriginalPrice,
                IsFeatured = p.IsFeatured
            }).ToList(),
            PageIndex = products.PageIndex,
            TotalPages = products.TotalPages,
            SearchTerm = search,
            MinPrice = minPrice,
            MaxPrice = maxPrice
        };

        return View(model);
    }
}
