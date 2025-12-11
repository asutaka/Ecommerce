using Ecommerce.Contracts;
using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Workers.Consumers;

public class ProcessPaymentConsumer(
    EcommerceDbContext dbContext,
    ILogger<ProcessPaymentConsumer> logger) : IConsumer<ProcessPayment>
{
    public async Task Consume(ConsumeContext<ProcessPayment> context)
    {
        var order = await dbContext.Orders
            .Include(x => x.Payment)
            .FirstOrDefaultAsync(x => x.Id == context.Message.OrderId, context.CancellationToken);

        if (order is null)
        {
            logger.LogWarning("Order {OrderId} not found for payment", context.Message.OrderId);
            return;
        }

        var payment = order.Payment ?? new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            Amount = context.Message.Amount,
            Method = context.Message.Method,
            CreatedAt = DateTime.UtcNow
        };

        payment.Status = PaymentStatus.Approved;
        payment.ProcessedAt = DateTime.UtcNow;
        payment.Reference = $"PMT-{payment.ProcessedAt:MMddHHmmss}";

        order.Payment = payment;
        order.Status = OrderStatus.Paid;

        dbContext.Payments.Update(payment);
        dbContext.Orders.Update(order);

        await dbContext.SaveChangesAsync(context.CancellationToken);

        await context.Publish(new PaymentProcessed(
            order.Id,
            payment.Status.ToString(),
            payment.ProcessedAt.Value,
            payment.Reference ?? string.Empty));

        logger.LogInformation("Payment processed for order {OrderId}", order.Id);
    }
}

