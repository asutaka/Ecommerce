using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductsController(EcommerceDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? searchTerm, int? pageNumber)
    {
        var query = dbContext.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(term) || 
                                     x.Description.ToLower().Contains(term));
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        int pageSize = 10;
        var products = await Models.PaginatedList<Infrastructure.Entities.Product>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize);

        ViewBag.SearchTerm = searchTerm;
        return View(products);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ProductFormViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Filter out empty URLs
        var validImages = model.ImageUrls.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        if (!validImages.Any())
        {
            ModelState.AddModelError("ImageUrls", "Vui lòng nhập ít nhất 1 link ảnh");
            return View(model);
        }

        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            IsFeatured = model.IsFeatured,
            Images = validImages
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var product = await dbContext.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var model = new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            IsFeatured = product.IsFeatured,
            ImageUrls = new List<string>(product.Images)
        };

        // Pad with empty strings to ensure 5 inputs
        while (model.ImageUrls.Count < 5)
        {
            model.ImageUrls.Add(string.Empty);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, ProductFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var product = await dbContext.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        // Filter out empty URLs
        var validImages = model.ImageUrls.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        if (!validImages.Any())
        {
            ModelState.AddModelError("ImageUrls", "Vui lòng nhập ít nhất 1 link ảnh");
            return View(model);
        }

        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = model.Price;
        product.IsFeatured = model.IsFeatured;
        product.Images = validImages;
        product.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await dbContext.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
