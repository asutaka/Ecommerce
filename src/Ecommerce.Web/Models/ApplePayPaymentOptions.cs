namespace Ecommerce.Web.Models;

public class ApplePayPaymentOptions
{
    public const string SectionName = "ApplePayPayment";
    
    public string MerchantId { get; set; } = string.Empty;
    public string MerchantName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = "VN";
    public string CurrencyCode { get; set; } = "VND";
    public bool UseMockService { get; set; } = true;
}

public class ApplePayPaymentRequest
{
    public string MerchantId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentData { get; set; } = string.Empty; // Encrypted payment token from Apple
}

public class ApplePayPaymentResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string PaymentUrl { get; set; } = string.Empty;
}

public class ApplePayCallbackRequest
{
    public string OrderId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    
    public bool IsSuccess => Status == "SUCCESS";
}
