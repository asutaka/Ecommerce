using Ecommerce.Infrastructure.Entities;
using Ecommerce.Web.Models;

namespace Ecommerce.Web.Services;

public interface IApplePayPaymentService
{
    Task<ApplePayPaymentResponse> CreatePaymentAsync(Order order);
    bool VerifyPaymentToken(string paymentData);
}

public class ApplePayMockPaymentService : IApplePayPaymentService
{
    private readonly ILogger<ApplePayMockPaymentService> _logger;

    public ApplePayMockPaymentService(ILogger<ApplePayMockPaymentService> logger)
    {
        _logger = logger;
    }

    public Task<ApplePayPaymentResponse> CreatePaymentAsync(Order order)
    {
        _logger.LogInformation("Creating mock Apple Pay payment for order {OrderId}", order.Id);

        var response = new ApplePayPaymentResponse
        {
            Success = true,
            Message = "Payment initiated",
            TransactionId = Guid.NewGuid().ToString("N"),
            PaymentUrl = $"/Payment/MockApplePayPayment?orderId={order.Id}&amount={order.Total}"
        };

        return Task.FromResult(response);
    }

    public bool VerifyPaymentToken(string paymentData)
    {
        // Mock always returns true
        _logger.LogInformation("Mock Apple Pay token verification (always true)");
        return true;
    }
}
