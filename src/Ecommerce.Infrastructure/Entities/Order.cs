namespace Ecommerce.Infrastructure.Entities;

public class Order : BaseEntity
{
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.PendingInvoice;
    public decimal Total { get; set; }
    
    // Optional: Link to registered customer
    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public Payment? Payment { get; set; }
    public NotificationLog? Notification { get; set; }
}

public class OrderItem : BaseEntity
{
    public Guid ProductId { get; set; }
    
    // Optional variant reference
    public Guid? ProductVariantId { get; set; }
    
    public required string ProductName { get; set; }
    
    // Variant snapshot for historical record
    public string? VariantSKU { get; set; }
    public string? VariantColor { get; set; }
    public string? VariantSize { get; set; }
    
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;
}

public enum OrderStatus
{
    PendingInvoice = 1,
    Invoiced = 2,
    PaymentProcessing = 3,
    Paid = 4,
    Notified = 5,
    Completed = 6,
    Cancelled = 98,
    Failed = 99
}
