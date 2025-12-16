namespace Ecommerce.Infrastructure.Entities;

public class Customer : BaseEntity
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    // Shipping addresses
    public string? ShippingAddress1 { get; set; }
    public string? ShippingAddress2 { get; set; }
    public string? ShippingAddress3 { get; set; }
    
    // Navigation properties
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
    public ICollection<ExternalLogin> ExternalLogins { get; set; } = new List<ExternalLogin>();
}
