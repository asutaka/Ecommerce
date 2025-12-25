using System.Diagnostics;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Models;
using Ecommerce.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Web.Controllers;

public class HomeController(
    ILogger<HomeController> logger,
    EcommerceDbContext dbContext,
    IMemoryCache cache) : Controller
{
    [HttpGet]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "*" })] // Cache for 5 minutes
    public async Task<IActionResult> Index()
    {
        var cacheKey = "home_page_data";
        
        if (!cache.TryGetValue(cacheKey, out HomeViewModel? model))
        {
        // 1. Featured Products - Optimized
        // - Added AsNoTracking() for read-only performance
        // - Removed Include(Category) as it's not used in projection
        // - Added Take(12) to prevent loading entire database
        var products = await dbContext.Products
            .Where(x => x.IsActive)
            .AsNoTracking()
            .OrderByDescending(x => x.IsFeatured)
            .ThenBy(x => x.Name)
            .Take(12) 
            .Select(x => new ProductViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Images = x.Images,
                Price = x.Price,
                IsFeatured = x.IsFeatured
            })
            .ToListAsync();

        // 2. Featured Categories - Optimized
        var categories = await dbContext.Categories
            .AsNoTracking()
            .Select(c => new CategorySummaryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductCount = dbContext.ProductCategories.Count(pc => pc.CategoryId == c.Id)
            })
            .OrderByDescending(c => c.ProductCount)
            .Take(6)
            .ToListAsync();

        // 3. New Arrivals - Optimized
        var newArrivals = await dbContext.Products
            .Where(x => x.IsActive)
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Take(4)
            .Select(x => new ProductViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Images = x.Images,
                Price = x.Price,
                IsFeatured = x.IsFeatured
            })
            .ToListAsync();

        // Get New Arrivals by Category
        var newArrivalsByCategory = new Dictionary<string, List<ProductViewModel>>();
        
        // Get root categories (ParentId = null)
        var collectionNames = await dbContext.Categories
            .Where(c => c.ParentId == null)
            .OrderByDescending(c => c.Priority)
            .ThenBy(c => c.Name)
            .Select(c => c.Name)
            .ToListAsync();

        
        foreach (var catName in collectionNames)
        {
            // Find category by name (searching both English and Vietnamese)
            var cat = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name.Contains(catName) || c.Name.Contains(catName == "Women" ? "Nữ" : catName == "Men" ? "Nam" : catName == "Kids" ? "Trẻ em" : "Phụ kiện"));
            
            if (cat != null)
            {
                var catProducts = await dbContext.Products
                    .Where(p => p.IsActive && p.ProductCategories.Any(pc => pc.CategoryId == cat.Id))
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(8)
                    .Select(x => new ProductViewModel
                    {
                         Id = x.Id,
                         Name = x.Name,
                         Description = x.Description,
                         Images = x.Images,
                         Price = x.Price,
                         IsFeatured = x.IsFeatured
                    })
                    .ToListAsync();
                
                newArrivalsByCategory[catName] = catProducts;
            }
        }


        var statistics = new HomeStatistics
        {
            TotalProducts = await dbContext.Products.CountAsync(),
            TotalCategories = await dbContext.Categories.CountAsync(),
            TotalOrders = await dbContext.Orders.CountAsync()
        };

        model = new HomeViewModel
        {
            FeaturedProducts = products,
            Checkout = new CheckoutViewModel
            {
                ProductId = products.FirstOrDefault()?.Id ?? Guid.Empty
            },
            FeaturedCategories = categories,
            NewArrivals = newArrivals,
            NewArrivalsByCategory = newArrivalsByCategory,
            Statistics = statistics
        };

            cache.Set(cacheKey, model, TimeSpan.FromMinutes(5));
        }

        logger.LogInformation("Rendering storefront with {ProductCount} sản phẩm", model.FeaturedProducts.Count);

        ViewBag.CheckoutStatus = TempData["CheckoutStatus"]?.ToString();

        return View(model);
    }

    [HttpGet]
    public IActionResult About()
    {
        var model = new AboutViewModel
        {
            Title = "Về Moderno",
            Description = "Chúng tôi tin rằng mọi người đều xứng đáng có những sản phẩm chất lượng cao, phong cách và giá cả hợp lý. Từ những xu hướng mới nhất đến những món đồ kinh điển vượt thời gian, mỗi sản phẩm được thiết kế với tình yêu, chọn lọc kỹ càng về chất lượng và phù hợp hoàn hảo với cuộc sống của bạn.",
            Statistics = new AboutStatistics
            {
                YearsInOperation = 10,
                CustomerRating = 4.9m,
                CustomerSatisfaction = 98
            },
            Features = new List<WhyChooseUs>
            {
                new WhyChooseUs
                {
                    Title = "Chất lượng cao cấp",
                    Description = "Mỗi sản phẩm được làm từ vải cao cấp và chất liệu thật.",
                    Icon = "bi-award"
                },
                new WhyChooseUs
                {
                    Title = "Cập nhật hàng tuần",
                    Description = "Phong cách mới trendy mỗi tuần, trực tiếp từ sàn diễn toàn cầu.",
                    Icon = "bi-lightning"
                },
                new WhyChooseUs
                {
                    Title = "Vừa vặn hoàn hảo",
                    Description = "Size đa dạng XS-4XL với hướng dẫn chi tiết cho mọi người.",
                    Icon = "bi-rulers"
                },
                new WhyChooseUs
                {
                    Title = "Giá cả hợp lý",
                    Description = "Giá trực tiếp từ nhà máy mang lại cảm giác cao cấp mà không tốn kém.",
                    Icon = "bi-tag"
                }
            }
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

