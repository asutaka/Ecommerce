using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Ecommerce.Infrastructure.Entities;
using Ecommerce.Web.Models;
using Microsoft.Extensions.Options;

namespace Ecommerce.Web.Services;

/// <summary>
/// Service for handling MoMo payment gateway integration
/// </summary>
public interface IMoMoPaymentService
{
    /// <summary>
    /// Create a payment request to MoMo
    /// </summary>
    /// <param name="order">Order to create payment for</param>
    /// <returns>MoMo payment response with payment URL</returns>
    Task<MoMoPaymentResponse> CreatePaymentAsync(Order order);
    
    /// <summary>
    /// Verify signature from MoMo callback
    /// </summary>
    /// <param name="ipnRequest">IPN request from MoMo</param>
    /// <returns>True if signature is valid</returns>
    bool VerifyIpnSignature(MoMoIpnRequest ipnRequest);
    
    /// <summary>
    /// Verify signature from MoMo return URL
    /// </summary>
    /// <param name="returnRequest">Return request from MoMo</param>
    /// <returns>True if signature is valid</returns>
    bool VerifyReturnSignature(MoMoReturnRequest returnRequest);
}

/// <summary>
/// Mock implementation of MoMo payment service for testing without real credentials
/// </summary>
public class MoMoMockPaymentService : IMoMoPaymentService
{
    private readonly MoMoPaymentOptions _options;
    private readonly ILogger<MoMoMockPaymentService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MoMoMockPaymentService(
        IOptions<MoMoPaymentOptions> options,
        ILogger<MoMoMockPaymentService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _options = options.Value;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<MoMoPaymentResponse> CreatePaymentAsync(Order order)
    {
        _logger.LogInformation("Creating MOCK MoMo payment for order {OrderId}", order.Id);

        var requestId = Guid.NewGuid().ToString();
        var orderId = order.Id.ToString();
        var amount = (long)order.Total;

        // Create mock payment URL that redirects to our mock payment page
        var baseUrl = GetBaseUrl();
        var mockPayUrl = $"{baseUrl}/Payment/MockMoMoPayment?orderId={orderId}&requestId={requestId}&amount={amount}";

        var response = new MoMoPaymentResponse
        {
            PartnerCode = "MOCK_PARTNER",
            RequestId = requestId,
            OrderId = orderId,
            Amount = amount,
            ResponseTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Message = "Successful (Mock)",
            ResultCode = 0,
            PayUrl = mockPayUrl,
            Deeplink = mockPayUrl,
            QrCodeUrl = ""
        };

        _logger.LogInformation("Mock MoMo payment created: {PayUrl}", mockPayUrl);

        return await Task.FromResult(response);
    }

    public bool VerifyIpnSignature(MoMoIpnRequest ipnRequest)
    {
        // In mock mode, always return true
        _logger.LogInformation("Verifying MOCK IPN signature for order {OrderId} - Always returns true", ipnRequest.OrderId);
        return true;
    }

    public bool VerifyReturnSignature(MoMoReturnRequest returnRequest)
    {
        // In mock mode, always return true
        _logger.LogInformation("Verifying MOCK return signature for order {OrderId} - Always returns true", returnRequest.OrderId);
        return true;
    }

    private string GetBaseUrl()
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
        {
            return "https://localhost:7001";
        }

        return $"{request.Scheme}://{request.Host}";
    }
}

/// <summary>
/// Real implementation of MoMo payment service
/// </summary>
public class MoMoPaymentService : IMoMoPaymentService
{
    private readonly MoMoPaymentOptions _options;
    private readonly ILogger<MoMoPaymentService> _logger;
    private readonly HttpClient _httpClient;

    public MoMoPaymentService(
        IOptions<MoMoPaymentOptions> options,
        ILogger<MoMoPaymentService> logger,
        HttpClient httpClient)
    {
        _options = options.Value;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<MoMoPaymentResponse> CreatePaymentAsync(Order order)
    {
        _logger.LogInformation("Creating MoMo payment for order {OrderId}", order.Id);

        var requestId = Guid.NewGuid().ToString();
        var orderId = order.Id.ToString();
        var amount = (long)order.Total;
        var orderInfo = $"Thanh toán đơn hàng #{order.Id}";
        var extraData = "";

        // Generate signature
        var rawSignature = $"accessKey={_options.AccessKey}" +
                          $"&amount={amount}" +
                          $"&extraData={extraData}" +
                          $"&ipnUrl={_options.IpnUrl}" +
                          $"&orderId={orderId}" +
                          $"&orderInfo={orderInfo}" +
                          $"&partnerCode={_options.PartnerCode}" +
                          $"&redirectUrl={_options.ReturnUrl}" +
                          $"&requestId={requestId}" +
                          $"&requestType=captureWallet";

        var signature = GenerateSignature(rawSignature, _options.SecretKey);

        var request = new MoMoPaymentRequest
        {
            PartnerCode = _options.PartnerCode,
            AccessKey = _options.AccessKey,
            RequestId = requestId,
            Amount = amount,
            OrderId = orderId,
            OrderInfo = orderInfo,
            RedirectUrl = _options.ReturnUrl,
            IpnUrl = _options.IpnUrl,
            RequestType = "captureWallet",
            ExtraData = extraData,
            Lang = "vi",
            Signature = signature
        };

        try
        {
            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending payment request to MoMo: {Endpoint}", _options.Endpoint);
            var httpResponse = await _httpClient.PostAsync(_options.Endpoint, content);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            _logger.LogInformation("MoMo response: {Response}", responseContent);

            var response = JsonSerializer.Deserialize<MoMoPaymentResponse>(responseContent);
            
            if (response == null)
            {
                throw new InvalidOperationException("Failed to deserialize MoMo response");
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating MoMo payment");
            throw;
        }
    }

    public bool VerifyIpnSignature(MoMoIpnRequest ipnRequest)
    {
        var rawSignature = $"accessKey={_options.AccessKey}" +
                          $"&amount={ipnRequest.Amount}" +
                          $"&extraData={ipnRequest.ExtraData}" +
                          $"&message={ipnRequest.Message}" +
                          $"&orderId={ipnRequest.OrderId}" +
                          $"&orderInfo={ipnRequest.OrderInfo}" +
                          $"&orderType={ipnRequest.OrderType}" +
                          $"&partnerCode={ipnRequest.PartnerCode}" +
                          $"&payType={ipnRequest.PayType}" +
                          $"&requestId={ipnRequest.RequestId}" +
                          $"&responseTime={ipnRequest.ResponseTime}" +
                          $"&resultCode={ipnRequest.ResultCode}" +
                          $"&transId={ipnRequest.TransId}";

        var expectedSignature = GenerateSignature(rawSignature, _options.SecretKey);
        var isValid = expectedSignature.Equals(ipnRequest.Signature, StringComparison.OrdinalIgnoreCase);

        _logger.LogInformation("IPN signature verification for order {OrderId}: {IsValid}", ipnRequest.OrderId, isValid);

        return isValid;
    }

    public bool VerifyReturnSignature(MoMoReturnRequest returnRequest)
    {
        var rawSignature = $"accessKey={_options.AccessKey}" +
                          $"&amount={returnRequest.Amount}" +
                          $"&extraData={returnRequest.ExtraData}" +
                          $"&message={returnRequest.Message}" +
                          $"&orderId={returnRequest.OrderId}" +
                          $"&orderInfo={returnRequest.OrderInfo}" +
                          $"&orderType={returnRequest.OrderType}" +
                          $"&partnerCode={returnRequest.PartnerCode}" +
                          $"&payType={returnRequest.PayType}" +
                          $"&requestId={returnRequest.RequestId}" +
                          $"&responseTime={returnRequest.ResponseTime}" +
                          $"&resultCode={returnRequest.ResultCode}" +
                          $"&transId={returnRequest.TransId}";

        var expectedSignature = GenerateSignature(rawSignature, _options.SecretKey);
        var isValid = expectedSignature.Equals(returnRequest.Signature, StringComparison.OrdinalIgnoreCase);

        _logger.LogInformation("Return signature verification for order {OrderId}: {IsValid}", returnRequest.OrderId, isValid);

        return isValid;
    }

    private static string GenerateSignature(string rawData, string secretKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var dataBytes = Encoding.UTF8.GetBytes(rawData);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
