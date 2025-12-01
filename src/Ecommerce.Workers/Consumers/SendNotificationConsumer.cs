using Ecommerce.Contracts;
using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Workers.Consumers;

public class SendNotificationConsumer(
    EcommerceDbContext dbContext,
    ILogger<SendNotificationConsumer> logger) : IConsumer<SendNotification>
{
    public async Task Consume(ConsumeContext<SendNotification> context)
    {
        var order = await dbContext.Orders
            .Include(x => x.Notification)
            .FirstOrDefaultAsync(x => x.Id == context.Message.OrderId, context.CancellationToken);

        if (order is null)
        {
            logger.LogWarning("Order {OrderId} not found for notification", context.Message.OrderId);
            return;
        }

        var notification = new NotificationLog
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            Channel = "email",
            Destination = context.Message.CustomerEmail,
            Template = context.Message.Template,
            SentAt = DateTime.UtcNow,
            Success = true
        };

        dbContext.Notifications.Add(notification);
        order.Status = OrderStatus.Completed;
        await dbContext.SaveChangesAsync(context.CancellationToken);

        await context.Publish(new NotificationSent(order.Id, notification.Channel, notification.SentAt));

        logger.LogInformation("Notification stored for order {OrderId}", order.Id);
    }
}

