namespace Ecommerce.Web.ViewModels;

public class ProductDetailViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();
    public decimal Price { get; set; }
    public bool IsFeatured { get; set; }
    
    // Category information
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    
    // Related products
    public List<ProductViewModel> RelatedProducts { get; set; } = new();
}
