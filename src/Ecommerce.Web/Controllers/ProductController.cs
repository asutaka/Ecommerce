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
            .Include(x => x.PrimaryCategory)
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
                OriginalPrice = rnd.Next(600, 800) * 10000, // Fake original price
                IsFeatured = true,
                PrimaryCategoryId = Guid.Empty,
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

            // Add fake coupons
            fakeModel.AvailableCoupons = new List<CouponViewModel>
            {
                new CouponViewModel
                {
                    Code = "GIAM40K",
                    Description = "Giảm 40.000đ cho đơn hàng từ 500.000đ",
                    DiscountAmount = 40000m,
                    MinimumOrderAmount = 500000m,
                    EndDate = DateTime.UtcNow.AddDays(30)
                },
                new CouponViewModel
                {
                    Code = "GIAM50K",
                    Description = "Giảm 50.000đ cho đơn hàng từ 800.000đ",
                    DiscountAmount = 50000m,
                    MinimumOrderAmount = 800000m,
                    EndDate = DateTime.UtcNow.AddDays(45)
                },
                new CouponViewModel
                {
                    Code = "GIAM100K",
                    Description = "Giảm 100.000đ cho đơn hàng từ 1.500.000đ",
                    DiscountAmount = 100000m,
                    MinimumOrderAmount = 1500000m,
                    EndDate = DateTime.UtcNow.AddDays(60)
                },
                new CouponViewModel
                {
                    Code = "FREESHIP",
                    Description = "Miễn phí vận chuyển cho đơn hàng từ 300.000đ",
                    DiscountAmount = 30000m,
                    MinimumOrderAmount = 300000m,
                    EndDate = DateTime.UtcNow.AddDays(90)
                }
            };

            return View(fakeModel);
        }

        // Get related products from same category
        var relatedProducts = await dbContext.Products
            .Where(x => x.PrimaryCategoryId == product.PrimaryCategoryId && x.Id != product.Id)
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

        // Get active coupons
        var now = DateTime.UtcNow;
        var coupons = await dbContext.Coupons
            .Where(x => x.IsActive && x.StartDate <= now && x.EndDate >= now && x.UsedCount < x.UsageLimit)
            .OrderBy(x => x.MinimumOrderAmount)
            .Select(c => new CouponViewModel
            {
                Code = c.Code,
                Description = c.Description,
                DiscountAmount = c.DiscountAmount,
                DiscountPercentage = c.DiscountPercentage,
                MinimumOrderAmount = c.MinimumOrderAmount,
                EndDate = c.EndDate
            })
            .ToListAsync();

        // If no coupons in database, use fake data
        if (!coupons.Any())
        {
            coupons = new List<CouponViewModel>
            {
                new CouponViewModel
                {
                    Code = "GIAM40K",
                    Description = "Giảm 40.000đ cho đơn hàng từ 500.000đ",
                    DiscountAmount = 40000m,
                    MinimumOrderAmount = 500000m,
                    EndDate = DateTime.UtcNow.AddDays(30)
                },
                new CouponViewModel
                {
                    Code = "GIAM50K",
                    Description = "Giảm 50.000đ cho đơn hàng từ 800.000đ",
                    DiscountAmount = 50000m,
                    MinimumOrderAmount = 800000m,
                    EndDate = DateTime.UtcNow.AddDays(45)
                },
                new CouponViewModel
                {
                    Code = "GIAM100K",
                    Description = "Giảm 100.000đ cho đơn hàng từ 1.500.000đ",
                    DiscountAmount = 100000m,
                    MinimumOrderAmount = 1500000m,
                    EndDate = DateTime.UtcNow.AddDays(60)
                },
                new CouponViewModel
                {
                    Code = "FREESHIP",
                    Description = "Miễn phí vận chuyển cho đơn hàng từ 300.000đ",
                    DiscountAmount = 30000m,
                    MinimumOrderAmount = 300000m,
                    EndDate = DateTime.UtcNow.AddDays(90)
                }
            };
        }

        // Load product variants
        var variants = await dbContext.ProductVariants
            .Where(v => v.ProductId == product.Id && v.IsActive)
            .OrderBy(v => v.Color)
            .ThenBy(v => v.Size)
            .Select(v => new ProductVariantViewModel
            {
                Id = v.Id,
                SKU = v.SKU,
                Color = v.Color,
                Size = v.Size,
                Stock = v.Stock,
                Price = v.Price,
                OriginalPrice = v.OriginalPrice,
                ImageUrls = v.ImageUrls,
                IsActive= v.IsActive
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
            OriginalPrice = product.OriginalPrice,
            IsFeatured = product.IsFeatured,
            PrimaryCategoryId = product.PrimaryCategoryId ?? Guid.Empty,
            CategoryName = product.PrimaryCategory?.Name ?? "N/A",
            RelatedProducts = relatedProducts,
            Variants = variants,
            AvailableCoupons = coupons
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
                .Include(x => x.PrimaryCategory)
                .Where(x => 
                    x.Name.ToLower().Contains(searchTerm) || 
                    x.Description.ToLower().Contains(searchTerm) ||
                    (x.PrimaryCategory != null && x.PrimaryCategory.Name.ToLower().Contains(searchTerm)))
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
