using Ecommerce.Contracts;
using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Infrastructure.Services;
using Hangfire;
using MassTransit;

namespace Ecommerce.Workers.Consumers;

/// <summary>
/// Consumer for processing payment commands
/// </summary>
public class PaymentConsumer : IConsumer<ProcessPaymentCommand>
{
    private readonly EcommerceDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IBackgroundJobClient _hangfireClient;
    private readonly ILogger<PaymentConsumer> _logger;

    public PaymentConsumer(
        EcommerceDbContext dbContext,
        IPublishEndpoint publishEndpoint,
        IBackgroundJobClient hangfireClient,
        ILogger<PaymentConsumer> logger)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
        _hangfireClient = hangfireClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProcessPaymentCommand> context)
    {
        var command = context.Message;
        _logger.LogInformation(
            "Processing payment for order {OrderId}, provider: {Provider}, attempt: {Attempt}",
            command.OrderId, command.PaymentProvider, command.AttemptNumber);

        var order = await _dbContext.Orders.FindAsync(command.OrderId);
        
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", command.OrderId);
            return;
        }

        // Check retry policy
        if (!PaymentRetryPolicy.CanRetryPayment(order, out string reason))
        {
            _logger.LogWarning(
                "Cannot retry payment for order {OrderId}: {Reason}",
                command.OrderId, reason);
            return;
        }

        // Increment attempt counter
        PaymentRetryPolicy.IncrementAttempt(order);
        order.PaymentProvider = command.PaymentProvider;
        
        try
        {
            // Simulate payment processing
            // In real implementation, call actual payment service
            var success = await SimulatePaymentProcessing(order, command.PaymentProvider);
            
            if (success)
            {
                await HandlePaymentSuccess(context, order);
            }
            else
            {
                await HandlePaymentFailure(context, order, "Payment gateway rejected");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for order {OrderId}", order.Id);
            await HandlePaymentFailure(context, order, ex.Message);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    private async Task<bool> SimulatePaymentProcessing(Order order, string provider)
    {
        // TODO: Replace with actual payment service call
        // For now, simulate with random success/failure
        await Task.Delay(1000); // Simulate API call
        
        // 70% success rate for simulation
        return Random.Shared.Next(100) < 70;
    }

    private async Task HandlePaymentSuccess(ConsumeContext<ProcessPaymentCommand> context, Order order)
    {
        _logger.LogInformation("Payment succeeded for order {OrderId}", order.Id);
        
        // Update order
        order.Status = OrderStatus.Paid;
        order.PaymentDate = DateTime.UtcNow;
        order.NextRetryScheduledAt = null;
        
        // Publish success event
        await context.Publish(new PaymentSucceededEvent
        {
            OrderId = order.Id,
            TransactionId = Guid.NewGuid().ToString("N"),
            Amount = order.Total,
            PaymentProvider = order.PaymentProvider ?? "Unknown",
            PaidAt = DateTime.UtcNow
        });
    }

    private async Task HandlePaymentFailure(
        ConsumeContext<ProcessPaymentCommand> context,
        Order order,
        string errorMessage)
    {
        var shouldRetry = order.PaymentAttempts < PaymentRetryPolicy.MAX_AUTO_RETRIES;
        
        if (shouldRetry)
        {
            // Calculate delay with exponential backoff
            var delay = PaymentRetryPolicy.CalculateRetryDelay(order.PaymentAttempts);
            var nextRetryAt = DateTime.UtcNow.Add(delay);
            
            order.NextRetryScheduledAt = nextRetryAt;
            
            _logger.LogInformation(
                "Scheduling retry #{Attempt} for order {OrderId} in {Delay} minutes",
                order.PaymentAttempts + 1, order.Id, delay.TotalMinutes);
            
            // Schedule delayed retry using Hangfire
            _hangfireClient.Schedule(
                () => PublishRetryCommand(order.Id, context.Message.PaymentProvider),
                delay);
            
            // Publish failed event (will retry)
            await context.Publish(new PaymentFailedEvent
            {
                OrderId = order.Id,
                Reason = errorMessage,
                AttemptNumber = order.PaymentAttempts,
                WillRetry = true,
                NextRetryAt = nextRetryAt,
                PaymentProvider = order.PaymentProvider ?? "Unknown"
            });
        }
        else
        {
            // Max retries reached - mark as failed
            order.Status = OrderStatus.Failed;
            order.IsDeleted = true;
            order.ExpiresAt = DateTime.UtcNow.AddDays(7);
            order.Note = $"Payment failed after {order.PaymentAttempts} attempts: {errorMessage}";
            order.NextRetryScheduledAt = null;
            
            _logger.LogWarning(
                "Payment failed permanently for order {OrderId} after {Attempts} attempts",
                order.Id, order.PaymentAttempts);
            
            // Publish failed event (no retry)
            await context.Publish(new PaymentFailedEvent
            {
                OrderId = order.Id,
                Reason = errorMessage,
                AttemptNumber = order.PaymentAttempts,
                WillRetry = false,
                NextRetryAt = null,
                PaymentProvider = order.PaymentProvider ?? "Unknown"
            });
        }
    }

    /// <summary>
    /// Hangfire job to publish retry command
    /// </summary>
    public async Task PublishRetryCommand(Guid orderId, string paymentProvider)
    {
        _logger.LogInformation("Publishing retry command for order {OrderId}", orderId);
        
        await _publishEndpoint.Publish(new ProcessPaymentCommand
        {
            OrderId = orderId,
            PaymentProvider = paymentProvider,
            AttemptNumber = 0 // Will be incremented by consumer
        });
    }
}
