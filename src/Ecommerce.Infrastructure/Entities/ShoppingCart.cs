namespace Ecommerce.Infrastructure.Entities;

public class ShoppingCart : BaseEntity
{
    /// <summary>
    /// Session ID for guest users (before login)
    /// </summary>
    public required string SessionId { get; set; }

    /// <summary>
    /// User ID for logged-in users (nullable for future authentication)
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Collection of items in this cart
    /// </summary>
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
