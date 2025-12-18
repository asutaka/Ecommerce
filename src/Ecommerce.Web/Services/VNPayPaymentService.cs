using Ecommerce.Infrastructure.Entities;
using Ecommerce.Web.Models;

namespace Ecommerce.Web.Services;

public interface IVNPayPaymentService
{
    Task<VNPayPaymentResponse> CreatePaymentAsync(Order order);
    bool VerifyCallbackSignature(VNPayCallbackRequest request);
}

public class VNPayMockPaymentService : IVNPayPaymentService
{
    private readonly ILogger<VNPayMockPaymentService> _logger;

    public VNPayMockPaymentService(ILogger<VNPayMockPaymentService> logger)
    {
        _logger = logger;
    }

    public Task<VNPayPaymentResponse> CreatePaymentAsync(Order order)
    {
        _logger.LogInformation("Creating mock VNPay payment for order {OrderId}", order.Id);

        var response = new VNPayPaymentResponse
        {
            PaymentUrl = $"/Payment/MockVNPayPayment?orderId={order.Id}&amount={order.Total}"
        };

        return Task.FromResult(response);
    }

    public bool VerifyCallbackSignature(VNPayCallbackRequest request)
    {
        // Mock always returns true
        _logger.LogInformation("Mock VNPay signature verification (always true)");
        return true;
    }
}
