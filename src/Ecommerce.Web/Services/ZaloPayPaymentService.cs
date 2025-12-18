using Ecommerce.Infrastructure.Entities;
using Ecommerce.Web.Models;

namespace Ecommerce.Web.Services;

public interface IZaloPayPaymentService
{
    Task<ZaloPayPaymentResponse> CreatePaymentAsync(Order order);
    bool VerifyCallbackSignature(ZaloPayCallbackRequest request);
}

public class ZaloPayMockPaymentService : IZaloPayPaymentService
{
    private readonly ILogger<ZaloPayMockPaymentService> _logger;

    public ZaloPayMockPaymentService(ILogger<ZaloPayMockPaymentService> logger)
    {
        _logger = logger;
    }

    public Task<ZaloPayPaymentResponse> CreatePaymentAsync(Order order)
    {
        _logger.LogInformation("Creating mock ZaloPay payment for order {OrderId}", order.Id);

        var response = new ZaloPayPaymentResponse
        {
            ReturnCode = 1,
            ReturnMessage = "Success",
            OrderUrl = $"/Payment/MockZaloPayPayment?orderId={order.Id}&amount={order.Total}",
            ZpTransToken = Guid.NewGuid().ToString("N")
        };

        return Task.FromResult(response);
    }

    public bool VerifyCallbackSignature(ZaloPayCallbackRequest request)
    {
        // Mock always returns true
        _logger.LogInformation("Mock ZaloPay signature verification (always true)");
        return true;
    }
}
