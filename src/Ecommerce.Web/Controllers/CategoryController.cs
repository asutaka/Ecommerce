using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Models;
using Ecommerce.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Controllers;

public class CategoryController(EcommerceDbContext dbContext) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(Guid id, int? pageNumber)
    {
        var category = await dbContext.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        var query = dbContext.Products
            .Where(x => x.CategoryId == id)
            .OrderByDescending(x => x.IsFeatured)
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
            ProductCount = await dbContext.Products.CountAsync(x => x.CategoryId == id),
            Products = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Images = p.Images,
                Price = p.Price,
                IsFeatured = p.IsFeatured
            }).ToList(),
            PageIndex = products.PageIndex,
            TotalPages = products.TotalPages
        };

        return View(model);
    }
}
