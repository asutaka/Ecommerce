using System.Diagnostics;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Models;
using Ecommerce.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Controllers;

public class HomeController(
    ILogger<HomeController> logger,
    EcommerceDbContext dbContext) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var products = await dbContext.Products
            .Include(x => x.Category)
            .OrderByDescending(x => x.IsFeatured)
            .ThenBy(x => x.Name)
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

        // Get featured categories (top 6 by product count)
        var categories = await dbContext.Categories
            .Select(c => new CategorySummaryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductCount = dbContext.Products.Count(p => p.CategoryId == c.Id)
            })
            .OrderByDescending(c => c.ProductCount)
            .Take(6)
            .ToListAsync();

        // Get new arrivals (latest 4 products)
        var newArrivals = await dbContext.Products
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

        // Get statistics
        var statistics = new HomeStatistics
        {
            TotalProducts = await dbContext.Products.CountAsync(),
            TotalCategories = await dbContext.Categories.CountAsync(),
            TotalOrders = await dbContext.Orders.CountAsync()
        };

        var model = new HomeViewModel
        {
            FeaturedProducts = products,
            Checkout = new CheckoutViewModel
            {
                ProductId = products.FirstOrDefault()?.Id ?? Guid.Empty
            },
            FeaturedCategories = categories,
            NewArrivals = newArrivals,
            Statistics = statistics
        };

        logger.LogInformation("Rendering storefront with {ProductCount} sản phẩm", products.Count);

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
