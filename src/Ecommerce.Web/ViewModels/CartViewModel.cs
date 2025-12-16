namespace Ecommerce.Web.ViewModels;

public class CartViewModel
{
    public Guid CartId { get; set; }
    public List<CartItemViewModel> Items { get; set; } = new();
    public string? AppliedCouponCode { get; set; }
    public decimal Discount { get; set; }
    public decimal Subtotal => Items.Sum(x => x.LineTotal);
    public decimal Tax => 0; // Can be calculated based on business rules
    public decimal Total => Subtotal + Tax - Discount;
    public int ItemCount => Items.Sum(x => x.Quantity);
}

public class CartItemViewModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string ProductImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}
