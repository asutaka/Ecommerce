using Ecommerce.Contracts;
using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Workers.Consumers;

public class CreateInvoiceConsumer(
    EcommerceDbContext dbContext,
    ILogger<CreateInvoiceConsumer> logger) : IConsumer<CreateInvoice>
{
    public async Task Consume(ConsumeContext<CreateInvoice> context)
    {
        var message = context.Message;

        var existing = await dbContext.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == message.OrderId, context.CancellationToken);

        if (existing is not null)
        {
            logger.LogInformation("Order {OrderId} already exists", message.OrderId);
            return;
        }

        var order = new Order
        {
            Id = message.OrderId,
            CustomerName = message.CustomerName,
            CustomerEmail = message.CustomerEmail,
            Status = OrderStatus.PendingInvoice,
            Total = message.Lines.Sum(x => x.UnitPrice * x.Quantity)
        };

        order.Items = message.Lines.Select(line => new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            ProductId = line.ProductId,
            ProductName = line.ProductName,
            UnitPrice = line.UnitPrice,
            Quantity = line.Quantity,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        await context.Publish(new InvoiceCreated(
            order.Id,
            $"INV-{DateTime.UtcNow:yyyyMMddHHmm}",
            order.Total,
            DateTime.UtcNow));

        logger.LogInformation("Order {OrderId} persisted and invoice event dispatched", order.Id);
    }
}

