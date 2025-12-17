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
    
    // Optional: Original price for showing discounts
    public decimal? OriginalPrice { get; set; }
    
    // Optional: Variant-specific images (multiple)
    public List<string> ImageUrls { get; set; } = new();
    
    public bool IsActive { get; set; } = true;
}
