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
                Description = "Góc ngồi sang trọng với chất liệu da Ý cao cấp.",
                HeroImageUrl = "https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60",
                Price = 420m,
                IsFeatured = true,
                CreatedAt = baseline
            },
            new Product
            {
                Id = Guid.Parse("f5d9d030-8cde-41e4-ac7c-7e27c856223c"),
                Name = "Canvas Lighting Kit",
                Description = "Bộ đèn canvas cân bằng ánh sáng tự nhiên.",
                HeroImageUrl = "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60",
                Price = 289m,
                IsFeatured = true,
                CreatedAt = baseline.AddDays(1)
            },
            new Product
            {
                Id = Guid.Parse("c812781e-3cc3-4cef-a0aa-3b40ffde4f74"),
                Name = "Nordic Marble Table",
                Description = "Bàn ăn đá cẩm thạch với đường nét tối giản.",
                HeroImageUrl = "https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60",
                Price = 980m,
                IsFeatured = false,
                CreatedAt = baseline.AddDays(2)
            }
        };

        modelBuilder.Entity<Product>().HasData(heroProducts);
    }
}

