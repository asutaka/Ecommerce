namespace Ecommerce.Infrastructure.Entities;

public class Product : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string HeroImageUrl { get; set; }
    public decimal Price { get; set; }
    public bool IsFeatured { get; set; }
}

