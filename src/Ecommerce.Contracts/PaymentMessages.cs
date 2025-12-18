namespace Ecommerce.Contracts;

/// <summary>
/// Command to process payment for an order
/// </summary>
public record ProcessPaymentCommand
{
    public Guid OrderId { get; init; }
    public string PaymentProvider { get; init; } = string.Empty; // MoMo, VNPay, ZaloPay, ApplePay
    public int AttemptNumber { get; init; } = 1;
    public DateTime ScheduledAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Event published when payment succeeds
/// </summary>
public record PaymentSucceededEvent
{
    public Guid OrderId { get; init; }
    public string TransactionId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string PaymentProvider { get; init; } = string.Empty;
    public DateTime PaidAt { get; init; }
}

/// <summary>
/// Event published when payment fails
/// </summary>
public record PaymentFailedEvent
{
    public Guid OrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public int AttemptNumber { get; init; }
    public bool WillRetry { get; init; }
    public DateTime? NextRetryAt { get; init; }
    public string PaymentProvider { get; init; } = string.Empty;
}

/// <summary>
/// Command to manually retry payment
/// </summary>
public record RetryPaymentCommand
{
    public Guid OrderId { get; init; }
    public string PaymentProvider { get; init; } = string.Empty;
}
