using MassTransit;

namespace Ecommerce.Workers.Sagas;

public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public Guid OrderId { get; set; }
    public string CurrentState { get; set; } = string.Empty;
    public string? InvoiceNumber { get; set; }
    public string? PaymentReference { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? NotifiedAt { get; set; }
}

