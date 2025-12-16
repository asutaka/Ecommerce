namespace Ecommerce.Infrastructure.Entities;

public class ShoppingCart : BaseEntity
{
    /// <summary>
    /// Session ID for guest users (before login)
    /// </summary>
    public required string SessionId { get; set; }

    /// <summary>
    /// Customer ID for logged-in users (nullable for guest checkout)
    /// </summary>
    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    /// <summary>
    /// Applied coupon code (if any)
    /// </summary>
    public string? AppliedCouponCode { get; set; }

    /// <summary>
    /// Discount amount from applied coupon
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Collection of items in this cart
    /// </summary>
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
