using Ecommerce.Infrastructure.Entities;
using Ecommerce.Web.Models;

namespace Ecommerce.Web.Areas.Admin.ViewModels;

public class OrderListViewModel
{
    public PaginatedList<Order> Orders { get; set; } = default!;
    public OrderStatus? CurrentStatus { get; set; }
    public string? Keyword { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class OrderDetailsViewModel
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<OrderItemViewModel> Items { get; set; } = new();
    public PaymentViewModel? Payment { get; set; }
    public NotificationLogViewModel? Notification { get; set; }
}

public class OrderItemViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public class PaymentViewModel
{
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
}

public class NotificationLogViewModel
{
    public string Channel { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public string Message { get; set; } = string.Empty;
}
