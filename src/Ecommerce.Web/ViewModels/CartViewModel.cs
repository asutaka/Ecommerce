namespace Ecommerce.Web.ViewModels;

public class CartViewModel
{
    public Guid CartId { get; set; }
    public List<CartItemViewModel> Items { get; set; } = new();
    public string? AppliedCouponCode { get; set; }
    public decimal Discount { get; set; }
    
    // Available coupons to display
    public List<CouponViewModel> AvailableCoupons { get; set; } = new();
    
    // Shipping info
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Note { get; set; }
    public List<string> AvailableAddresses { get; set; } = new();
    
    // Payment
    public string PaymentMethod { get; set; } = "COD";
    public decimal ShippingFee { get; set; } = 30000; // Default 30k
    
    // Calculations
    public decimal Subtotal => Items.Sum(x => x.LineTotal);
    public decimal OriginalSubtotal => Items.Sum(x => (x.OriginalPrice ?? x.UnitPrice) * x.Quantity);
    public decimal Tax => 0; // Can be calculated based on business rules
    public decimal Total => Subtotal + Tax + ShippingFee - Discount;
    public int ItemCount => Items.Sum(x => x.Quantity);
}

public class CartItemViewModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string ProductImageUrl { get; set; }
    public string? VariantColor { get; set; }
    public string? VariantSize { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}
