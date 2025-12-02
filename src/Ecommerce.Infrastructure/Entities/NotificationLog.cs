namespace Ecommerce.Infrastructure.Entities;

public class NotificationLog : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;
    public required string Channel { get; set; }
    public required string Destination { get; set; }
    public string Template { get; set; } = "order-confirmation";
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool Success { get; set; } = true;
}

