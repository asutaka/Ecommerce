namespace Ecommerce.Infrastructure.Entities;

public class ExternalLogin : BaseEntity
{
    public required string Provider { get; set; } // Facebook, Twitter, Google
    public required string ProviderKey { get; set; } // Unique ID from provider
    public required Guid CustomerId { get; set; }
    
    // Navigation property
    public Customer Customer { get; set; } = default!;
}
