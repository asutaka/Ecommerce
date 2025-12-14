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
            // FAKE DATA FALLBACK (For UI Verification)
            // If product not found in DB, generate a dummy one so we can see the UI
            var rnd = new Random();
            var fakeModel = new ProductDetailViewModel
            {
                Id = id,
                Name = "Sản phẩm Demo " + id.ToString().Substring(0, 4),
                Description = "Đây là dữ liệu giả lập để kiểm tra giao diện chi tiết sản phẩm. Sản phẩm này chưa có trong cơ sở dữ liệu thực tế.",
                SKU = "MOD-DEMO-" + rnd.Next(1000, 9999),
                Price = rnd.Next(50, 500) * 10000,
                IsFeatured = true,
                CategoryId = Guid.Empty,
                CategoryName = "Danh mục Demo",
                Images = new List<string> 
                { 
                    "https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?auto=format&fit=crop&w=800&q=80",
                    "https://images.unsplash.com/photo-1529139574466-a302d20525a9?auto=format&fit=crop&w=800&q=80",
                    "https://images.unsplash.com/photo-1483985988355-763728e1935b?auto=format&fit=crop&w=800&q=80"
                },
                RelatedProducts = new List<ProductViewModel>()
            };

            // Add some fake related products
            for(int i=0; i<4; i++) {
                fakeModel.RelatedProducts.Add(new ProductViewModel {
                    Id = Guid.NewGuid(),
                    Name = $"Sản phẩm gợi ý {i+1}",
                    Price = 1500000,
                    Images = new List<string> { "https://images.unsplash.com/photo-1509631179647-0177331693ae?auto=format&fit=crop&w=800&q=80" }
                });
            }

            return View(fakeModel);
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
            SKU = "MOD-" + product.Id.ToString().Substring(0, 8).ToUpper(),
            Images = product.Images,
            Price = product.Price,
            IsFeatured = product.IsFeatured,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? "N/A",
            RelatedProducts = relatedProducts
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        var model = new SearchResultViewModel
        {
            Query = query ?? string.Empty
        };

        if (!string.IsNullOrWhiteSpace(query))
        {
            var searchTerm = query.Trim().ToLower();
            
            var products = await dbContext.Products
                .Include(x => x.Category)
                .Where(x => 
                    x.Name.ToLower().Contains(searchTerm) || 
                    x.Description.ToLower().Contains(searchTerm) ||
                    (x.Category != null && x.Category.Name.ToLower().Contains(searchTerm)))
                .OrderByDescending(x => x.IsFeatured)
                .ThenBy(x => x.Name)
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

            model.Results = products;
        }

        return View(model);
    }
}
