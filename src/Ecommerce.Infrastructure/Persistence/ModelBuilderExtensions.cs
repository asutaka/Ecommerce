using Ecommerce.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence;

public static class ModelBuilderExtensions
{
    public static void SeedInitialData(this ModelBuilder modelBuilder)
    {
        var baseline = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        var heroProducts = new[]
        {
            new Product
            {
                Id = Guid.Parse("dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10"),
                Name = "Moderno Leather Chair",
                Slug = "moderno-leather-chair-dc1c2d",
                Description = "Góc ngồi sang trọng với chất liệu da Ý cao cấp.",
                Images = new List<string> { "https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60" },
                Price = 420m,
                IsFeatured = true,
                CreatedAt = baseline
            },
            new Product
            {
                Id = Guid.Parse("f5d9d030-8cde-41e4-ac7c-7e27c856223c"),
                Name = "Canvas Lighting Kit",
                Slug = "canvas-lighting-kit-f5d9d0",
                Description = "Bộ đèn canvas cân bằng ánh sáng tự nhiên.",
                Images = new List<string> { "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60" },
                Price = 289m,
                IsFeatured = true,
                CreatedAt = baseline.AddDays(1)
            },
            new Product
            {
                Id = Guid.Parse("c812781e-3cc3-4cef-a0aa-3b40ffde4f74"),
                Name = "Nordic Marble Table",
                Slug = "nordic-marble-table-c81278",
                Description = "Bàn ăn đá cẩm thạch với đường nét tối giản.",
                Images = new List<string> { "https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60" },
                Price = 980m,
                IsFeatured = false,
                CreatedAt = baseline.AddDays(2)
            }
        };

        modelBuilder.Entity<Product>().HasData(heroProducts);
    }

    public static void SeedCouponData(this ModelBuilder modelBuilder)
    {
        var now = DateTime.UtcNow;
        var coupons = new[]
        {
            new Coupon
            {
                Id = Guid.NewGuid(),
                Code = "GIAM40K",
                Description = "Giảm 40.000đ cho đơn hàng từ 500.000đ",
                DiscountAmount = 40000m,
                MinimumOrderAmount = 500000m,
                StartDate = now.AddDays(-7),
                EndDate = now.AddDays(30),
                IsActive = true,
                UsageLimit = 100,
                UsedCount = 0,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Coupon
            {
                Id = Guid.NewGuid(),
                Code = "GIAM50K",
                Description = "Giảm 50.000đ cho đơn hàng từ 800.000đ",
                DiscountAmount = 50000m,
                MinimumOrderAmount = 800000m,
                StartDate = now.AddDays(-5),
                EndDate = now.AddDays(45),
                IsActive = true,
                UsageLimit = 50,
                UsedCount = 0,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Coupon
            {
                Id = Guid.NewGuid(),
                Code = "GIAM100K",
                Description = "Giảm 100.000đ cho đơn hàng từ 1.500.000đ",
                DiscountAmount = 100000m,
                MinimumOrderAmount = 1500000m,
                StartDate = now.AddDays(-3),
                EndDate = now.AddDays(60),
                IsActive = true,
                UsageLimit = 30,
                UsedCount = 0,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Coupon
            {
                Id = Guid.NewGuid(),
                Code = "FREESHIP",
                Description = "Miễn phí vận chuyển cho đơn hàng từ 300.000đ",
                DiscountAmount = 30000m,
                MinimumOrderAmount = 300000m,
                StartDate = now.AddDays(-10),
                EndDate = now.AddDays(90),
                IsActive = true,
                UsageLimit = 200,
                UsedCount = 0,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        modelBuilder.Entity<Coupon>().HasData(coupons);
    }
}

