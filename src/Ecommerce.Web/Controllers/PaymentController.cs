using Ecommerce.Contracts;
using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Models;
using Ecommerce.Web.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Controllers;

/// <summary>
/// Controller for handling payment operations
/// </summary>
public class PaymentController : Controller
{
    private readonly IMoMoPaymentService _momoService;
    private readonly IZaloPayPaymentService _zaloPayService;
    private readonly IVNPayPaymentService _vnPayService;
    private readonly IApplePayPaymentService _applePayService;
    private readonly EcommerceDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IMoMoPaymentService momoService,
        IZaloPayPaymentService zaloPayService,
        IVNPayPaymentService vnPayService,
        IApplePayPaymentService applePayService,
        EcommerceDbContext dbContext,
        IPublishEndpoint publishEndpoint,
        ILogger<PaymentController> logger)
    {
        _momoService = momoService;
        _zaloPayService = zaloPayService;
        _vnPayService = vnPayService;
        _applePayService = applePayService;
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    /// <summary>
    /// Initiate MoMo payment for an order
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> InitiateMoMoPayment(Guid orderId)
    {
        try
        {
            var order = await _dbContext.Orders.FindAsync(orderId);
            
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", orderId);
                TempData["Error"] = "Không tìm thấy đơn hàng";
                return RedirectToAction("Index", "Cart");
            }

            // Update order status
            order.Status = OrderStatus.PaymentProcessing;
            order.PaymentProvider = "MoMo";
            await _dbContext.SaveChangesAsync();

            // Publish payment command to queue for async processing
            await _publishEndpoint.Publish(new ProcessPaymentCommand
            {
                OrderId = order.Id,
                PaymentProvider = "MoMo",
                AttemptNumber = 1
            });

            _logger.LogInformation("Published payment command for order {OrderId}", orderId);

            // Redirect to processing page
            return RedirectToAction("ProcessingPayment", new { orderId, provider = "MoMo" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating MoMo payment for order {OrderId}", orderId);
            TempData["Error"] = "Có lỗi xảy ra khi tạo thanh toán. Vui lòng thử lại.";
            return RedirectToAction("Index", "Cart");
        }
    }

    /// <summary>
    /// Show payment processing page
    /// </summary>
    [HttpGet]
    public IActionResult ProcessingPayment(Guid orderId, string provider)
    {
        ViewBag.OrderId = orderId;
        ViewBag.Provider = provider;
        return View();
    }

    /// <summary>
    /// Handle IPN (Instant Payment Notification) from MoMo
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> MoMoCallback([FromBody] MoMoIpnRequest ipnRequest)
    {
        _logger.LogInformation("Received MoMo IPN callback for order {OrderId}", ipnRequest.OrderId);

        try
        {
            // Verify signature
            if (!_momoService.VerifyIpnSignature(ipnRequest))
            {
                _logger.LogWarning("Invalid IPN signature for order {OrderId}", ipnRequest.OrderId);
                return Ok(new { resultCode = 97, message = "Invalid signature" });
            }

            // Find order
            if (!Guid.TryParse(ipnRequest.OrderId, out var orderId))
            {
                _logger.LogWarning("Invalid order ID format: {OrderId}", ipnRequest.OrderId);
                return Ok(new { resultCode = 99, message = "Invalid order ID" });
            }

            var order = await _dbContext.Orders.FindAsync(orderId);
            
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found in IPN callback", orderId);
                return Ok(new { resultCode = 99, message = "Order not found" });
            }

            // Update order based on payment result
            if (ipnRequest.IsSuccess)
            {
                order.Status = OrderStatus.Paid;
                order.MoMoTransactionId = ipnRequest.TransId.ToString();
                order.PaymentDate = DateTime.UtcNow;
                
                _logger.LogInformation("Payment successful for order {OrderId}, TransId: {TransId}", 
                    orderId, ipnRequest.TransId);
            }
            else
            {
                // Payment failed - soft delete with TTL
                _logger.LogWarning("Payment failed for order {OrderId}: {Message}", 
                    orderId, ipnRequest.Message);
                
                order.Status = OrderStatus.Failed;
                order.Note = $"Payment failed: {ipnRequest.Message}";
                order.ExpiresAt = DateTime.UtcNow.AddDays(7); // Auto-delete after 7 days
                order.IsDeleted = true;
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new { resultCode = 0, message = "Success" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MoMo IPN callback");
            return Ok(new { resultCode = 99, message = "Internal error" });
        }
    }

    /// <summary>
    /// Handle return from MoMo payment page
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> MoMoReturn([FromQuery] MoMoReturnRequest returnRequest)
    {
        _logger.LogInformation("User returned from MoMo for order {OrderId}", returnRequest.OrderId);

        try
        {
            // Verify signature
            if (!_momoService.VerifyReturnSignature(returnRequest))
            {
                _logger.LogWarning("Invalid return signature for order {OrderId}", returnRequest.OrderId);
                TempData["Error"] = "Xác thực thanh toán không hợp lệ";
                return RedirectToAction("Index", "Cart");
            }

            if (!Guid.TryParse(returnRequest.OrderId, out var orderId))
            {
                _logger.LogWarning("Invalid order ID format: {OrderId}", returnRequest.OrderId);
                TempData["Error"] = "Mã đơn hàng không hợp lệ";
                return RedirectToAction("Index", "Cart");
            }

            var order = await _dbContext.Orders.FindAsync(orderId);

            if (returnRequest.IsSuccess)
            {
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found but payment was successful", orderId);
                    TempData["Warning"] = "Thanh toán thành công nhưng không tìm thấy đơn hàng. Vui lòng liên hệ CSKH.";
                    return RedirectToAction("Index", "Home");
                }

                // Payment successful - redirect to confirmation
                TempData["Success"] = "Thanh toán thành công!";
                return RedirectToAction("Confirmation", "Cart", new { orderId });
            }
            else
            {
                // Payment failed - order should have been deleted by IPN callback
                // But if it still exists, delete it now
                if (order != null)
                {
                    _dbContext.Orders.Remove(order);
                    await _dbContext.SaveChangesAsync();
                }

                _logger.LogWarning("Payment failed for order {OrderId}: {Message}", 
                    orderId, returnRequest.Message);
                
                TempData["Error"] = $"Thanh toán thất bại: {returnRequest.Message}. Vui lòng thử lại.";
                return RedirectToAction("Index", "Cart");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MoMo return");
            TempData["Error"] = "Có lỗi xảy ra khi xử lý kết quả thanh toán";
            return RedirectToAction("Index", "Cart");
        }
    }

    /// <summary>
    /// Mock MoMo payment page for testing
    /// </summary>
    [HttpGet]
    public IActionResult MockMoMoPayment(string orderId, string requestId, long amount)
    {
        ViewBag.OrderId = orderId;
        ViewBag.RequestId = requestId;
        ViewBag.Amount = amount;
        
        return View();
    }

    /// <summary>
    /// Process mock payment result
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ProcessMockPayment(string orderId, string requestId, long amount, bool success)
    {
        _logger.LogInformation("Processing mock payment for order {OrderId}: {Success}", orderId, success);

        try
        {
            // Simulate IPN callback
            var ipnRequest = new MoMoIpnRequest
            {
                PartnerCode = "MOCK_PARTNER",
                OrderId = orderId,
                RequestId = requestId,
                Amount = amount,
                OrderInfo = $"Thanh toán đơn hàng #{orderId}",
                OrderType = "momo_wallet",
                TransId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                ResultCode = success ? 0 : 1000,
                Message = success ? "Successful" : "Payment failed",
                PayType = "qr",
                ResponseTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                ExtraData = "",
                Signature = "mock_signature"
            };

            // Process the payment
            await MoMoCallback(ipnRequest);

            // Simulate return URL
            var returnRequest = new MoMoReturnRequest
            {
                PartnerCode = "MOCK_PARTNER",
                OrderId = orderId,
                RequestId = requestId,
                Amount = amount,
                OrderInfo = $"Thanh toán đơn hàng #{orderId}",
                OrderType = "momo_wallet",
                TransId = ipnRequest.TransId,
                ResultCode = ipnRequest.ResultCode,
                Message = ipnRequest.Message,
                PayType = "qr",
                ResponseTime = ipnRequest.ResponseTime,
                ExtraData = "",
                Signature = "mock_signature"
            };

            return await MoMoReturn(returnRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing mock payment");
            TempData["Error"] = "Có lỗi xảy ra khi xử lý thanh toán mock";
            return RedirectToAction("Index", "Cart");
        }
    }

    #region ZaloPay Payment

/// <summary>
/// Initiate ZaloPay payment for an order
/// </summary>
[HttpGet]
public async Task<IActionResult> InitiateZaloPayPayment(Guid orderId)
{
    try
    {
        var order = await _dbContext.Orders.FindAsync(orderId);
        
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", orderId);
            TempData["Error"] = "KhÃ´ng tÃ¬m tháº¥y Ä‘Æ¡n hÃ ng";
            return RedirectToAction("Index", "Cart");
        }

        var paymentResponse = await _zaloPayService.CreatePaymentAsync(order);

        if (paymentResponse.ReturnCode != 1)
        {
            _logger.LogError("Failed to create ZaloPay payment for order {OrderId}: {Message}", 
                orderId, paymentResponse.ReturnMessage);
            
            order.Status = OrderStatus.Failed;
            order.ExpiresAt = DateTime.UtcNow.AddDays(7);
            order.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            
            TempData["Error"] = $"KhÃ´ng thá»ƒ táº¡o thanh toÃ¡n: {paymentResponse.ReturnMessage}";
            return RedirectToAction("Index", "Cart");
        }

        order.Status = OrderStatus.PaymentProcessing;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Redirecting to ZaloPay payment URL for order {OrderId}", orderId);
        return Redirect(paymentResponse.OrderUrl);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error initiating ZaloPay payment for order {OrderId}", orderId);
        TempData["Error"] = "CÃ³ lá»—i xáº£y ra khi táº¡o thanh toÃ¡n. Vui lÃ²ng thá»­ láº¡i.";
        return RedirectToAction("Index", "Cart");
    }
}

[HttpGet]
public IActionResult MockZaloPayPayment(string orderId, long amount)
{
    ViewBag.OrderId = orderId;
    ViewBag.Amount = amount;
    ViewBag.Provider = "ZaloPay";
    ViewBag.ProviderLogo = "/images/zalopay-logo.png";
    
    return View("MockPayment");
}

[HttpPost]
public async Task<IActionResult> ProcessMockZaloPayPayment(string orderId, long amount, bool success)
{
    _logger.LogInformation("Processing mock ZaloPay payment for order {OrderId}: {Success}", orderId, success);

    try
    {
        if (!Guid.TryParse(orderId, out var orderGuid))
        {
            TempData["Error"] = "MÃ£ Ä‘Æ¡n hÃ ng khÃ´ng há»£p lá»‡";
            return RedirectToAction("Index", "Cart");
        }

        var order = await _dbContext.Orders.FindAsync(orderGuid);
        
        if (order == null)
        {
            TempData["Error"] = "KhÃ´ng tÃ¬m tháº¥y Ä‘Æ¡n hÃ ng";
            return RedirectToAction("Index", "Cart");
        }

        if (success)
        {
            order.Status = OrderStatus.Paid;
            order.PaymentDate = DateTime.UtcNow;
            TempData["Success"] = "Thanh toÃ¡n ZaloPay thÃ nh cÃ´ng!";
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Confirmation", "Cart", new { orderId = orderGuid });
        }
        else
        {
            order.Status = OrderStatus.Failed;
            order.Note = "ZaloPay payment failed";
            order.ExpiresAt = DateTime.UtcNow.AddDays(7);
            order.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            
            TempData["Error"] = "Thanh toÃ¡n ZaloPay tháº¥t báº¡i. Vui lÃ²ng thá»­ láº¡i.";
            return RedirectToAction("Index", "Cart");
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error processing mock ZaloPay payment");
        TempData["Error"] = "CÃ³ lá»—i xáº£y ra khi xá»­ lÃ½ thanh toÃ¡n";
        return RedirectToAction("Index", "Cart");
    }
}

#endregion

#region VNPay Payment

/// <summary>
/// Initiate VNPay payment for an order
/// </summary>
[HttpGet]
public async Task<IActionResult> InitiateVNPayPayment(Guid orderId)
{
    try
    {
        var order = await _dbContext.Orders.FindAsync(orderId);
        
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", orderId);
            TempData["Error"] = "KhÃ´ng tÃ¬m tháº¥y Ä‘Æ¡n hÃ ng";
            return RedirectToAction("Index", "Cart");
        }

        var paymentResponse = await _vnPayService.CreatePaymentAsync(order);

        order.Status = OrderStatus.PaymentProcessing;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Redirecting to VNPay payment URL for order {OrderId}", orderId);
        return Redirect(paymentResponse.PaymentUrl);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error initiating VNPay payment for order {OrderId}", orderId);
        TempData["Error"] = "CÃ³ lá»—i xáº£y ra khi táº¡o thanh toÃ¡n. Vui lÃ²ng thá»­ láº¡i.";
        return RedirectToAction("Index", "Cart");
    }
}

[HttpGet]
public IActionResult MockVNPayPayment(string orderId, decimal amount)
{
    ViewBag.OrderId = orderId;
    ViewBag.Amount = amount;
    ViewBag.Provider = "VNPay";
    ViewBag.ProviderLogo = "/images/vnpay-logo.png";
    
    return View("MockPayment");
}

[HttpPost]
public async Task<IActionResult> ProcessMockVNPayPayment(string orderId, decimal amount, bool success)
{
    _logger.LogInformation("Processing mock VNPay payment for order {OrderId}: {Success}", orderId, success);

    try
    {
        if (!Guid.TryParse(orderId, out var orderGuid))
        {
            TempData["Error"] = "MÃ£ Ä‘Æ¡n hÃ ng khÃ´ng há»£p lá»‡";
            return RedirectToAction("Index", "Cart");
        }

        var order = await _dbContext.Orders.FindAsync(orderGuid);
        
        if (order == null)
        {
            TempData["Error"] = "KhÃ´ng tÃ¬m tháº¥y Ä‘Æ¡n hÃ ng";
            return RedirectToAction("Index", "Cart");
        }

        if (success)
        {
            order.Status = OrderStatus.Paid;
            order.PaymentDate = DateTime.UtcNow;
            TempData["Success"] = "Thanh toÃ¡n VNPay thÃ nh cÃ´ng!";
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Confirmation", "Cart", new { orderId = orderGuid });
        }
        else
        {
            order.Status = OrderStatus.Failed;
            order.Note = "VNPay payment failed";
            order.ExpiresAt = DateTime.UtcNow.AddDays(7);
            order.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            
            TempData["Error"] = "Thanh toÃ¡n VNPay tháº¥t báº¡i. Vui lÃ²ng thá»­ láº¡i.";
            return RedirectToAction("Index", "Cart");
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error processing mock VNPay payment");
        TempData["Error"] = "CÃ³ lá»—i xáº£y ra khi xá»­ lÃ½ thanh toÃ¡n";
        return RedirectToAction("Index", "Cart");
    }
}

#endregion

#region Apple Pay Payment

/// <summary>
/// Initiate Apple Pay payment for an order
/// </summary>
[HttpGet]
public async Task<IActionResult> InitiateApplePayPayment(Guid orderId)
{
    try
    {
        var order = await _dbContext.Orders.FindAsync(orderId);
        
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", orderId);
            TempData["Error"] = "KhÃ´ng tÃ¬m tháº¥y Ä‘Æ¡n hÃ ng";
            return RedirectToAction("Index", "Cart");
        }

        var paymentResponse = await _applePayService.CreatePaymentAsync(order);

        if (!paymentResponse.Success)
        {
            _logger.LogError("Failed to create Apple Pay payment for order {OrderId}: {Message}", 
                orderId, paymentResponse.Message);
            
            order.Status = OrderStatus.Failed;
            order.ExpiresAt = DateTime.UtcNow.AddDays(7);
            order.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            
            TempData["Error"] = $"KhÃ´ng thá»ƒ táº¡o thanh toÃ¡n: {paymentResponse.Message}";
            return RedirectToAction("Index", "Cart");
        }

        order.Status = OrderStatus.PaymentProcessing;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Redirecting to Apple Pay payment URL for order {OrderId}", orderId);
        return Redirect(paymentResponse.PaymentUrl);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error initiating Apple Pay payment for order {OrderId}", orderId);
        TempData["Error"] = "CÃ³ lá»—i xáº£y ra khi táº¡o thanh toÃ¡n. Vui lÃ²ng thá»­ láº¡i.";
        return RedirectToAction("Index", "Cart");
    }
}

[HttpGet]
public IActionResult MockApplePayPayment(string orderId, decimal amount)
{
    ViewBag.OrderId = orderId;
    ViewBag.Amount = amount;
    ViewBag.Provider = "Apple Pay";
    ViewBag.ProviderLogo = "/images/applepay-logo.png";
    
    return View("MockPayment");
}

[HttpPost]
public async Task<IActionResult> ProcessMockApplePayPayment(string orderId, decimal amount, bool success)
{
    _logger.LogInformation("Processing mock Apple Pay payment for order {OrderId}: {Success}", orderId, success);

    try
    {
        if (!Guid.TryParse(orderId, out var orderGuid))
        {
            TempData["Error"] = "MÃ£ Ä‘Æ¡n hÃ ng khÃ´ng há»£p lá»‡";
            return RedirectToAction("Index", "Cart");
        }

        var order = await _dbContext.Orders.FindAsync(orderGuid);
        
        if (order == null)
        {
            TempData["Error"] = "KhÃ´ng tÃ¬m tháº¥y Ä‘Æ¡n hÃ ng";
            return RedirectToAction("Index", "Cart");
        }

        if (success)
        {
            order.Status = OrderStatus.Paid;
            order.PaymentDate = DateTime.UtcNow;
            TempData["Success"] = "Thanh toÃ¡n Apple Pay thÃ nh cÃ´ng!";
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Confirmation", "Cart", new { orderId = orderGuid });
        }
        else
        {
            order.Status = OrderStatus.Failed;
            order.Note = "Apple Pay payment failed";
            order.ExpiresAt = DateTime.UtcNow.AddDays(7);
            order.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            
            TempData["Error"] = "Thanh toÃ¡n Apple Pay tháº¥t báº¡i. Vui lÃ²ng thá»­ láº¡i.";
            return RedirectToAction("Index", "Cart");
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error processing mock Apple Pay payment");
        TempData["Error"] = "CÃ³ lá»—i xáº£y ra khi xá»­ lÃ½ thanh toÃ¡n";
        return RedirectToAction("Index", "Cart");
    }
}

#endregion
}
