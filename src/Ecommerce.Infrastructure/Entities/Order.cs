namespace Ecommerce.Infrastructure.Entities;

public class Order : BaseEntity
{
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.PendingInvoice;
    public decimal Total { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public Payment? Payment { get; set; }
    public NotificationLog? Notification { get; set; }
}

public class OrderItem : BaseEntity
{
    public Guid ProductId { get; set; }
    public required string ProductName { get; set; }
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
    Failed = 99
}

