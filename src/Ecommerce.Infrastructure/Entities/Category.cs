namespace Ecommerce.Infrastructure.Entities;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Priority { get; set; } = 0; // 0 = lowest priority, higher number = higher priority
    
    public Guid? ParentId { get; set; }
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();

    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}
