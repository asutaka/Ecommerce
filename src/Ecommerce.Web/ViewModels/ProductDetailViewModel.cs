namespace Ecommerce.Web.ViewModels;

public class ProductDetailViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public bool IsFeatured { get; set; }
    
    // Category information
    public Guid PrimaryCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    
    // Related products
    public List<ProductViewModel> RelatedProducts { get; set; } = new();
    
    // Product variants
    public List<ProductVariantViewModel> Variants { get; set; } = new();
    
    // Available coupons
    public List<CouponViewModel> AvailableCoupons { get; set; } = new();
}

public class ProductVariantViewModel
{
    public Guid Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string? Size { get; set; }
    public int Stock { get; set; }
    public decimal? Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public bool IsActive { get; set; }
}
