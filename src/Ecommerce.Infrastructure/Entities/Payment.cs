namespace Ecommerce.Infrastructure.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Method { get; set; } = "Card";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime? ProcessedAt { get; set; }
    public string? Reference { get; set; }
}

public enum PaymentStatus
{
    Pending = 1,
    Approved = 2,
    Declined = 3,
    Refunded = 4
}

