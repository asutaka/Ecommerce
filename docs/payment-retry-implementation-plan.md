# Implementation Plan: Advanced Payment Retry with RabbitMQ

## 🎯 Mục tiêu
Implement automatic payment retry với RabbitMQ + MassTransit:
- ✅ Auto retry với exponential backoff
- ✅ Delayed message scheduling
- ✅ Max 3 auto retries
- ✅ Manual retry option
- ✅ Notification khi payment done

---

## 📐 Architecture

```
┌─────────────────────────────────────────────────────┐
│                  Payment Flow                        │
├─────────────────────────────────────────────────────┤
│                                                      │
│  User clicks "Thanh toán"                           │
│         ↓                                            │
│  Publish: ProcessPaymentCommand                     │
│         ↓                                            │
│  [RabbitMQ Queue]                                   │
│         ↓                                            │
│  PaymentConsumer processes                          │
│         ↓                                            │
│  ┌─────────────┬──────────────┐                    │
│  │   Success   │    Failed    │                    │
│  └─────────────┴──────────────┘                    │
│         │              │                             │
│         │              ├─ Attempt < 3?              │
│         │              │   Yes: Schedule retry      │
│         │              │        (delayed message)   │
│         │              │   No: Mark as failed       │
│         │              │                             │
│         ↓              ↓                             │
│  Update Order    Publish: PaymentFailedEvent       │
│  Send notification                                   │
│                                                      │
└─────────────────────────────────────────────────────┘
```

---

## 🔧 Implementation Details

### 1. Message Contracts

**File**: `Contracts/PaymentMessages.cs`

```csharp
namespace Ecommerce.Contracts;

// Command: Process payment
public record ProcessPaymentCommand
{
    public Guid OrderId { get; init; }
    public string PaymentProvider { get; init; } // MoMo, VNPay, etc.
    public int AttemptNumber { get; init; } = 1;
    public DateTime ScheduledAt { get; init; } = DateTime.UtcNow;
}

// Event: Payment succeeded
public record PaymentSucceededEvent
{
    public Guid OrderId { get; init; }
    public string TransactionId { get; init; }
    public decimal Amount { get; init; }
    public DateTime PaidAt { get; init; }
}

// Event: Payment failed
public record PaymentFailedEvent
{
    public Guid OrderId { get; init; }
    public string Reason { get; init; }
    public int AttemptNumber { get; init; }
    public bool WillRetry { get; init; }
    public DateTime? NextRetryAt { get; init; }
}

// Command: Retry payment (manual)
public record RetryPaymentCommand
{
    public Guid OrderId { get; init; }
    public string PaymentProvider { get; init; }
}
```

---

### 2. Payment Consumer

**File**: `Workers/Consumers/PaymentConsumer.cs`

```csharp
public class PaymentConsumer : IConsumer<ProcessPaymentCommand>
{
    private readonly IPaymentServiceFactory _paymentFactory;
    private readonly EcommerceDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PaymentConsumer> _logger;

    public async Task Consume(ConsumeContext<ProcessPaymentCommand> context)
    {
        var command = context.Message;
        var order = await _dbContext.Orders.FindAsync(command.OrderId);
        
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", command.OrderId);
            return;
        }

        // Check retry policy
        if (!PaymentRetryPolicy.CanRetryPayment(order, out string reason))
        {
            _logger.LogWarning("Cannot retry payment for order {OrderId}: {Reason}", 
                command.OrderId, reason);
            return;
        }

        // Increment attempt
        order.PaymentAttempts++;
        order.LastPaymentAttempt = DateTime.UtcNow;
        
        try
        {
            // Get payment service
            var paymentService = _paymentFactory.GetService(command.PaymentProvider);
            
            // Process payment
            var result = await paymentService.ProcessPaymentAsync(order);
            
            if (result.IsSuccess)
            {
                // Success!
                order.Status = OrderStatus.Paid;
                order.PaymentDate = DateTime.UtcNow;
                order.MoMoTransactionId = result.TransactionId; // or other provider
                
                await _dbContext.SaveChangesAsync();
                
                // Publish success event
                await context.Publish(new PaymentSucceededEvent
                {
                    OrderId = order.Id,
                    TransactionId = result.TransactionId,
                    Amount = order.Total,
                    PaidAt = DateTime.UtcNow
                });
                
                _logger.LogInformation("Payment succeeded for order {OrderId}", order.Id);
            }
            else
            {
                // Failed - should we retry?
                await HandlePaymentFailure(context, order, result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for order {OrderId}", order.Id);
            await HandlePaymentFailure(context, order, ex.Message);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    private async Task HandlePaymentFailure(
        ConsumeContext<ProcessPaymentCommand> context,
        Order order,
        string errorMessage)
    {
        var command = context.Message;
        var shouldRetry = order.PaymentAttempts < PaymentRetryPolicy.MAX_AUTO_RETRIES;
        
        if (shouldRetry)
        {
            // Calculate delay with exponential backoff
            var delay = CalculateRetryDelay(order.PaymentAttempts);
            var nextRetryAt = DateTime.UtcNow.Add(delay);
            
            _logger.LogInformation(
                "Scheduling retry #{Attempt} for order {OrderId} in {Delay}",
                order.PaymentAttempts + 1, order.Id, delay);
            
            // Schedule delayed retry
            await context.ScheduleSend(
                nextRetryAt,
                new ProcessPaymentCommand
                {
                    OrderId = order.Id,
                    PaymentProvider = command.PaymentProvider,
                    AttemptNumber = order.PaymentAttempts + 1,
                    ScheduledAt = nextRetryAt
                });
            
            // Publish failed event (will retry)
            await context.Publish(new PaymentFailedEvent
            {
                OrderId = order.Id,
                Reason = errorMessage,
                AttemptNumber = order.PaymentAttempts,
                WillRetry = true,
                NextRetryAt = nextRetryAt
            });
        }
        else
        {
            // Max retries reached - mark as failed
            order.Status = OrderStatus.Failed;
            order.IsDeleted = true;
            order.ExpiresAt = DateTime.UtcNow.AddDays(7);
            order.Note = $"Payment failed after {order.PaymentAttempts} attempts: {errorMessage}";
            
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
                NextRetryAt = null
            });
        }
    }

    private TimeSpan CalculateRetryDelay(int attemptNumber)
    {
        // Exponential backoff: 5min, 15min, 30min
        return attemptNumber switch
        {
            1 => TimeSpan.FromMinutes(5),
            2 => TimeSpan.FromMinutes(15),
            _ => TimeSpan.FromMinutes(30)
        };
    }
}
```

---

### 3. Payment Service Factory

**File**: `Services/PaymentServiceFactory.cs`

```csharp
public interface IPaymentServiceFactory
{
    IPaymentService GetService(string provider);
}

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(Order order);
}

public class PaymentResult
{
    public bool IsSuccess { get; set; }
    public string? TransactionId { get; set; }
    public string? ErrorMessage { get; set; }
}

public class PaymentServiceFactory : IPaymentServiceFactory
{
    private readonly IMoMoPaymentService _momoService;
    private readonly IZaloPayPaymentService _zaloPayService;
    private readonly IVNPayPaymentService _vnPayService;
    private readonly IApplePayPaymentService _applePayService;

    public IPaymentService GetService(string provider)
    {
        return provider switch
        {
            "MoMo" => new MoMoPaymentServiceAdapter(_momoService),
            "ZaloPay" => new ZaloPayPaymentServiceAdapter(_zaloPayService),
            "VNPay" => new VNPayPaymentServiceAdapter(_vnPayService),
            "ApplePay" => new ApplePayPaymentServiceAdapter(_applePayService),
            _ => throw new ArgumentException($"Unknown payment provider: {provider}")
        };
    }
}
```

---

### 4. Notification Consumer

**File**: `Workers/Consumers/PaymentNotificationConsumer.cs`

```csharp
public class PaymentSuccessNotificationConsumer : IConsumer<PaymentSucceededEvent>
{
    private readonly IEmailService _emailService;
    
    public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
    {
        var evt = context.Message;
        
        // Send email notification
        await _emailService.SendPaymentSuccessEmailAsync(evt.OrderId);
        
        // Could also send SMS, push notification, etc.
    }
}

public class PaymentFailedNotificationConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly IEmailService _emailService;
    
    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var evt = context.Message;
        
        if (evt.WillRetry)
        {
            // Send "we're retrying" notification
            await _emailService.SendPaymentRetryNotificationAsync(
                evt.OrderId, 
                evt.NextRetryAt.Value);
        }
        else
        {
            // Send "payment failed" notification
            await _emailService.SendPaymentFailedEmailAsync(evt.OrderId);
        }
    }
}
```

---

### 5. Update Payment Controllers

**File**: `Controllers/PaymentController.cs`

```csharp
public class PaymentController : Controller
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    [HttpGet]
    public async Task<IActionResult> InitiateMoMoPayment(Guid orderId)
    {
        // Instead of calling payment service directly,
        // publish command to queue
        await _publishEndpoint.Publish(new ProcessPaymentCommand
        {
            OrderId = orderId,
            PaymentProvider = "MoMo",
            AttemptNumber = 1
        });
        
        TempData["Info"] = "Đang xử lý thanh toán. Bạn sẽ nhận được thông báo khi hoàn tất.";
        return RedirectToAction("ProcessingPayment", new { orderId });
    }
    
    [HttpGet]
    public IActionResult ProcessingPayment(Guid orderId)
    {
        ViewBag.OrderId = orderId;
        return View(); // Show "Processing..." page with polling
    }
}
```

---

### 6. Database Changes

**Add to Order entity:**
```csharp
public int PaymentAttempts { get; set; } = 0;
public DateTime? LastPaymentAttempt { get; set; }
public DateTime? NextRetryScheduledAt { get; set; }
```

**Migration**: `AddPaymentRetryFields`

---

### 7. MassTransit Configuration

**File**: `Program.cs`

```csharp
builder.Services.AddMassTransit(x =>
{
    // Register consumers
    x.AddConsumer<PaymentConsumer>();
    x.AddConsumer<PaymentSuccessNotificationConsumer>();
    x.AddConsumer<PaymentFailedNotificationConsumer>();
    
    x.SetKebabCaseEndpointNameFormatter();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitSection = builder.Configuration.GetSection("RabbitMq");
        var host = rabbitSection.GetValue<string>("Host") ?? "localhost";
        var username = rabbitSection.GetValue<string>("Username") ?? "guest";
        var password = rabbitSection.GetValue<string>("Password") ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });
        
        // Enable delayed message scheduling
        cfg.UseDelayedMessageScheduler();
        
        cfg.ConfigureEndpoints(context);
    });
});

// Register payment service factory
builder.Services.AddScoped<IPaymentServiceFactory, PaymentServiceFactory>();
```

---

## 📊 Retry Strategy

### Exponential Backoff Schedule

| Attempt | Delay | Total Time |
|---------|-------|------------|
| 1 (initial) | 0 | 0 |
| 2 (auto) | 5 min | 5 min |
| 3 (auto) | 15 min | 20 min |
| 4 (auto) | 30 min | 50 min |
| 5+ (manual) | User action | - |

**After 3 auto retries:**
- Order marked as Failed
- Soft deleted (TTL 7 days)
- User can still manually retry from cart

---

## 🎨 UI Updates

### Processing Payment Page

**File**: `Views/Payment/ProcessingPayment.cshtml`

```html
<div class="text-center">
    <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
    </div>
    <h3>Đang xử lý thanh toán...</h3>
    <p>Vui lòng không đóng trang này</p>
    <p id="status">Đang kết nối với @ViewBag.Provider...</p>
</div>

<script>
// Poll for payment status
setInterval(async () => {
    const response = await fetch('/api/payment/status/@ViewBag.OrderId');
    const data = await response.json();
    
    if (data.status === 'Paid') {
        window.location.href = '/Cart/Confirmation?orderId=@ViewBag.OrderId';
    } else if (data.status === 'Failed') {
        window.location.href = '/Cart?error=payment-failed';
    }
}, 3000); // Poll every 3 seconds
</script>
```

---

## ✅ Implementation Checklist

### Phase 1: Setup (30 min)
- [ ] Add PaymentAttempts fields to Order
- [ ] Create migration
- [ ] Apply migration
- [ ] Install MassTransit.RabbitMQ.Scheduler (if needed)

### Phase 2: Message Contracts (15 min)
- [ ] Create ProcessPaymentCommand
- [ ] Create PaymentSucceededEvent
- [ ] Create PaymentFailedEvent

### Phase 3: Consumers (60 min)
- [ ] Create PaymentConsumer
- [ ] Implement retry logic with backoff
- [ ] Create notification consumers
- [ ] Test consumers

### Phase 4: Services (45 min)
- [ ] Create IPaymentServiceFactory
- [ ] Create adapters for each provider
- [ ] Update PaymentRetryPolicy

### Phase 5: Controllers & UI (40 min)
- [ ] Update payment initiation endpoints
- [ ] Create ProcessingPayment view
- [ ] Add status polling API
- [ ] Update Cart view for manual retry

### Phase 6: Testing (60 min)
- [ ] Test auto retry flow
- [ ] Test exponential backoff
- [ ] Test max retries
- [ ] Test manual retry
- [ ] Test notifications

**Total**: ~4 hours

---

## 🚀 Benefits

✅ **Professional**: Auto retry như các hệ thống lớn
✅ **Resilient**: Tự động recover từ transient errors
✅ **Scalable**: RabbitMQ handle high load
✅ **User-friendly**: Không cần user action cho retry
✅ **Observable**: Easy monitoring qua RabbitMQ dashboard

---

## 📝 Notes

- Cần RabbitMQ Delayed Message Plugin
- MassTransit đã có sẵn trong project
- Có thể dùng Hangfire thay vì RabbitMQ scheduler nếu muốn
