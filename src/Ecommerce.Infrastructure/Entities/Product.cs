namespace Ecommerce.Infrastructure.Entities;

public class Product : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public List<string> Images { get; set; } = new();
    public decimal Price { get; set; }
    public bool IsFeatured { get; set; }

    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }

    // Helper to maintain compatibility
    public string HeroImageUrl => Images.FirstOrDefault() ?? "https://placehold.co/600x400?text=No+Image";
}

