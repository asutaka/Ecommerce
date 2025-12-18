using Ecommerce.Contracts;
using MassTransit;

namespace Ecommerce.Workers.Consumers;

/// <summary>
/// Consumer for payment success notifications
/// </summary>
public class PaymentSuccessNotificationConsumer : IConsumer<PaymentSucceededEvent>
{
    private readonly ILogger<PaymentSuccessNotificationConsumer> _logger;

    public PaymentSuccessNotificationConsumer(ILogger<PaymentSuccessNotificationConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
    {
        var evt = context.Message;
        
        _logger.LogInformation(
            "Payment succeeded for order {OrderId}, amount: {Amount}, provider: {Provider}",
            evt.OrderId, evt.Amount, evt.PaymentProvider);
        
        // TODO: Send email notification
        // await _emailService.SendPaymentSuccessEmailAsync(evt.OrderId);
        
        // TODO: Send SMS notification
        // await _smsService.SendPaymentSuccessSmsAsync(evt.OrderId);
        
        await Task.CompletedTask;
    }
}

/// <summary>
/// Consumer for payment failure notifications
/// </summary>
public class PaymentFailedNotificationConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly ILogger<PaymentFailedNotificationConsumer> _logger;

    public PaymentFailedNotificationConsumer(ILogger<PaymentFailedNotificationConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var evt = context.Message;
        
        if (evt.WillRetry)
        {
            _logger.LogInformation(
                "Payment failed for order {OrderId}, will retry at {NextRetry}",
                evt.OrderId, evt.NextRetryAt);
            
            // TODO: Send "we're retrying" notification
            // await _emailService.SendPaymentRetryNotificationAsync(evt.OrderId, evt.NextRetryAt.Value);
        }
        else
        {
            _logger.LogWarning(
                "Payment failed permanently for order {OrderId}: {Reason}",
                evt.OrderId, evt.Reason);
            
            // TODO: Send "payment failed" notification
            // await _emailService.SendPaymentFailedEmailAsync(evt.OrderId);
        }
        
        await Task.CompletedTask;
    }
}
