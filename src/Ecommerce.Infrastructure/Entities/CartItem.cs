namespace Ecommerce.Infrastructure.Entities;

public class CartItem : BaseEntity
{
    /// <summary>
    /// Foreign key to ShoppingCart
    /// </summary>
    public Guid CartId { get; set; }

    /// <summary>
    /// Navigation property to ShoppingCart
    /// </summary>
    public ShoppingCart Cart { get; set; } = default!;

    /// <summary>
    /// Foreign key to Product
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Navigation property to Product
    /// </summary>
    public Product Product { get; set; } = default!;

    /// <summary>
    /// Foreign key to ProductVariant (nullable for backward compatibility)
    /// </summary>
    public Guid? ProductVariantId { get; set; }

    /// <summary>
    /// Navigation property to ProductVariant
    /// </summary>
    public ProductVariant? ProductVariant { get; set; }

    /// <summary>
    /// Snapshot of product name at time of adding to cart
    /// </summary>
    public required string ProductName { get; set; }

    /// <summary>
    /// Snapshot of variant info (Color, Size, SKU) for historical record
    /// </summary>
    public string? VariantSKU { get; set; }
    public string? VariantColor { get; set; }
    public string? VariantSize { get; set; }

    /// <summary>
    /// Snapshot of product image URL at time of adding to cart
    /// </summary>
    public required string ProductImageUrl { get; set; }

    /// <summary>
    /// Snapshot of product price at time of adding to cart
    /// (price may change later, but cart preserves original price)
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Snapshot of original price (before discount) at time of adding to cart
    /// </summary>
    public decimal? OriginalPrice { get; set; }

    /// <summary>
    /// Quantity of this product in the cart
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Calculated line total (UnitPrice * Quantity)
    /// </summary>
    public decimal LineTotal => UnitPrice * Quantity;
}
