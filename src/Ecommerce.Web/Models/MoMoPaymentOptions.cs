namespace Ecommerce.Web.Models;

/// <summary>
/// Configuration options for MoMo Payment Gateway
/// </summary>
public class MoMoPaymentOptions
{
    public const string SectionName = "MoMoPayment";
    
    /// <summary>
    /// Partner Code provided by MoMo
    /// </summary>
    public string PartnerCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Access Key provided by MoMo
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Secret Key for HMAC SHA256 signature
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
    
    /// <summary>
    /// MoMo API endpoint (sandbox or production)
    /// </summary>
    public string Endpoint { get; set; } = "https://test-payment.momo.vn/v2/gateway/api/create";
    
    /// <summary>
    /// URL where user will be redirected after payment
    /// </summary>
    public string ReturnUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// URL where MoMo will send IPN (Instant Payment Notification)
    /// </summary>
    public string IpnUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Enable mock mode for testing without real MoMo credentials
    /// </summary>
    public bool UseMockService { get; set; } = false;
}
