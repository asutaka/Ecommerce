using Ecommerce.Contracts;
using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Workers.Consumers;

public class CancelOrderConsumer(
    EcommerceDbContext dbContext,
    ILogger<CancelOrderConsumer> logger) : IConsumer<CancelOrder>
{
    public async Task Consume(ConsumeContext<CancelOrder> context)
    {
        var message = context.Message;

        var order = await dbContext.Orders.FindAsync([message.OrderId], context.CancellationToken);

        if (order is null)
        {
            logger.LogWarning("Order {OrderId} not found for cancellation", message.OrderId);
            return;
        }

        if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Failed)
        {
            logger.LogWarning("Order {OrderId} cannot be cancelled because it is in status {Status}", message.OrderId, order.Status);
            return;
        }

        order.Status = OrderStatus.Cancelled;
        // We could also log the reason if we had a field for it, or add a note.
        
        await dbContext.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation("Order {OrderId} cancelled. Reason: {Reason}", message.OrderId, message.Reason);
    }
}
