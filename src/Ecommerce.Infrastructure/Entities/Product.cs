namespace Ecommerce.Infrastructure.Entities;

public class Product : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public List<string> Images { get; set; } = new();
    public decimal Price { get; set; }
    public bool IsFeatured { get; set; }

    public Guid? PrimaryCategoryId { get; set; }
    public Category? PrimaryCategory { get; set; }

    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    // Helper to maintain compatibility
    public string HeroImageUrl => Images.FirstOrDefault() ?? "https://placehold.co/600x400?text=No+Image";
}

