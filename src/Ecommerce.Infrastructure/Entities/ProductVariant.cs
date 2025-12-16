namespace Ecommerce.Infrastructure.Entities;

public class ProductVariant : BaseEntity
{
    public required Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public required string SKU { get; set; } // Unique identifier
    
    public string? Color { get; set; }
    public string? Size { get; set; }
    
    public int Stock { get; set; } = 0;
    
    // Optional: Override product price if variant has different price
    public decimal? Price { get; set; }
    
    // Optional: Variant-specific image
    public string? ImageUrl { get; set; }
    
    public bool IsActive { get; set; } = true;
}
