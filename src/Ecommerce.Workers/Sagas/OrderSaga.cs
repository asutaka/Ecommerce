using Ecommerce.Contracts;
using MassTransit;

namespace Ecommerce.Workers.Sagas;

public class OrderSaga : MassTransitStateMachine<OrderState>
{
    public State InvoiceCreatedState { get; private set; } = null!;
    public State PaymentProcessedState { get; private set; } = null!;
    public State NotificationSentState { get; private set; } = null!;

    public Event<CreateInvoice> CreateInvoiceEvent { get; private set; } = null!;
    public Event<InvoiceCreated> InvoiceCreatedEvent { get; private set; } = null!;
    public Event<PaymentProcessed> PaymentProcessedEvent { get; private set; } = null!;
    public Event<NotificationSent> NotificationSentEvent { get; private set; } = null!;

    public OrderSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => CreateInvoiceEvent, cfg =>
        {
            cfg.CorrelateById(context => context.Message.OrderId);
            cfg.SelectId(_ => Guid.NewGuid());
        });

        Event(() => InvoiceCreatedEvent, cfg =>
        {
            cfg.CorrelateById(context => context.Message.OrderId);
        });

        Event(() => PaymentProcessedEvent, cfg =>
        {
            cfg.CorrelateById(context => context.Message.OrderId);
        });

        Event(() => NotificationSentEvent, cfg =>
        {
            cfg.CorrelateById(context => context.Message.OrderId);
        });

        Initially(
            When(CreateInvoiceEvent)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.CreatedAt = DateTime.UtcNow;
                })
                .TransitionTo(InvoiceCreatedState)
        );

        During(InvoiceCreatedState,
            When(InvoiceCreatedEvent)
                .Then(context => context.Saga.InvoiceNumber = context.Message.InvoiceNumber)
                .Publish(context => new ProcessPayment(context.Message.OrderId, context.Message.Total, "VietQR"))
                .TransitionTo(PaymentProcessedState)
        );

        During(PaymentProcessedState,
            When(PaymentProcessedEvent)
                .Then(context =>
                {
                    context.Saga.PaidAt = context.Message.ProcessedAt;
                    context.Saga.PaymentReference = context.Message.Reference;
                })
                .Publish(context => new SendNotification(
                    context.Message.OrderId,
                    context.Saga.InvoiceNumber ?? "unknown@moderno.com",
                    "order-confirmation",
                    "Đơn hàng của bạn đã thanh toán thành công"))
                .TransitionTo(NotificationSentState)
        );

        During(NotificationSentState,
            When(NotificationSentEvent)
                .Then(context => context.Saga.NotifiedAt = context.Message.SentAt)
                .Finalize());

        SetCompletedWhenFinalized();
    }
}

