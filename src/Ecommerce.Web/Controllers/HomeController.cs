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

        // Get New Arrivals by Category
        var targetCategories = new[] { "Women", "Men", "Kids", "Accessories", "Nữ", "Nam", "Trẻ em", "Phụ kiện" };
        var newArrivalsByCategory = new Dictionary<string, List<ProductViewModel>>();

        // Normalize naming for the tabs
        var tabMapping = new Dictionary<string, string>
        {
            { "Women", "Dresses" }, // Mapping Women to "Dresses" label as per image request or keep meaningful
            { "Nữ", "Dresses" },
            { "Men", "Shirts" },
            { "Nam", "Shirts" },
            { "Kids", "Jeans" }, // Just mapping to tabs as example, or use actual Category Names
            { "Trẻ em", "Jeans" },
            { "Accessories", "Tops" },
            { "Phụ kiện", "Tops" }
        };
        // Actually, user wants "Dresses", "Shirts", "Jeans", "Tops", "Blazers" as buttons? 
        // Or "Women", "Men" etc? The user said "mỗi button sẽ là tên của từng category từ mục collections".
        // Collections are: Women, Men, Kids, Accessories.
        // Let's stick to Women, Men, Kids, Accessories but maybe map them if needed. 
        // The user image shows "Dresses", "Shirts" etc.
        // User request: "mỗi button sẽ là tên của từng category từ mục collections" -> So buttons should be: Women, Men, Kids, Accessories.
        
        var collectionNames = new[] { "Women", "Men", "Kids", "Accessories" };
        
        foreach (var catName in collectionNames)
        {
            // Find category by name (searching both English and Vietnamese)
            var cat = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name.Contains(catName) || c.Name.Contains(catName == "Women" ? "Nữ" : catName == "Men" ? "Nam" : catName == "Kids" ? "Trẻ em" : "Phụ kiện"));
            
            if (cat != null)
            {
                var catProducts = await dbContext.Products
                    .Where(p => p.CategoryId == cat.Id)
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

        // FAKE DATA GENERATOR (For UI Verification)
        // If any category is empty, fill it with dummy data
        var rnd = new Random();
        foreach (var catName in collectionNames)
        {
            if (!newArrivalsByCategory.ContainsKey(catName) || !newArrivalsByCategory[catName].Any())
            {
                var fakeProducts = new List<ProductViewModel>();
                
                // Select image based on category
                string imgUrl = "https://placehold.co/600x400";
                if (catName == "Women" || catName == "Nữ") imgUrl = "https://images.unsplash.com/photo-1509631179647-0177331693ae?auto=format&fit=crop&w=800&q=80";
                else if (catName == "Men" || catName == "Nam") imgUrl = "https://images.unsplash.com/photo-1503342217505-b0a15ec3261c?auto=format&fit=crop&w=800&q=80";
                else if (catName == "Kids" || catName == "Trẻ em") imgUrl = "https://images.unsplash.com/photo-1504593811423-6dd665756598?auto=format&fit=crop&w=800&q=80";
                else if (catName == "Accessories" || catName == "Phụ kiện") imgUrl = "https://images.unsplash.com/photo-1509631179647-0177331693ae?auto=format&fit=crop&w=800&q=80&sat=-50";

                for (int i = 1; i <= 12; i++)
                {
                    // Generate 1-3 random images per product
                    var imageCount = rnd.Next(1, 4); // 1, 2, or 3 images
                    var productImages = new List<string>();
                    
                    for (int j = 0; j < imageCount; j++)
                    {
                        // Add slight variation to image URL to get different images
                        var variation = rnd.Next(1, 100);
                        productImages.Add($"{imgUrl}&sig={variation + j}");
                    }
                    
                    fakeProducts.Add(new ProductViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{catName} Item {i} (Fake)",
                        Description = "This is a generated product for testing UI layout.",
                        Price = rnd.Next(20, 200) * 10000,
                        Images = productImages,
                        IsFeatured = i % 2 == 0
                    });
                }
                newArrivalsByCategory[catName] = fakeProducts;
            }
        }

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
            NewArrivalsByCategory = newArrivalsByCategory,
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
