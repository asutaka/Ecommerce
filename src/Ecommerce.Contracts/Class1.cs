namespace Ecommerce.Contracts;

public record CatalogItem(Guid ProductId, string Name, decimal Price, string ImageUrl);

public record CreateInvoice(
    Guid OrderId,
    string CustomerName,
    string CustomerEmail,
    IReadOnlyCollection<OrderLine> Lines);

public record OrderLine(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity);

public record InvoiceCreated(Guid OrderId, string InvoiceNumber, decimal Total, DateTime CreatedAt);

public record ProcessPayment(Guid OrderId, decimal Amount, string Method);

public record PaymentProcessed(Guid OrderId, string Status, DateTime ProcessedAt, string Reference);

public record SendNotification(Guid OrderId, string CustomerEmail, string Template, string Subject);

public record NotificationSent(Guid OrderId, string Channel, DateTime SentAt);
