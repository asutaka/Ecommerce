using System.Text.Json.Serialization;

namespace Ecommerce.Web.Models;

/// <summary>
/// Request to create MoMo payment
/// </summary>
public class MoMoPaymentRequest
{
    [JsonPropertyName("partnerCode")]
    public string PartnerCode { get; set; } = string.Empty;
    
    [JsonPropertyName("accessKey")]
    public string AccessKey { get; set; } = string.Empty;
    
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;
    
    [JsonPropertyName("amount")]
    public long Amount { get; set; }
    
    [JsonPropertyName("orderId")]
    public string OrderId { get; set; } = string.Empty;
    
    [JsonPropertyName("orderInfo")]
    public string OrderInfo { get; set; } = string.Empty;
    
    [JsonPropertyName("redirectUrl")]
    public string RedirectUrl { get; set; } = string.Empty;
    
    [JsonPropertyName("ipnUrl")]
    public string IpnUrl { get; set; } = string.Empty;
    
    [JsonPropertyName("requestType")]
    public string RequestType { get; set; } = "captureWallet";
    
    [JsonPropertyName("extraData")]
    public string ExtraData { get; set; } = string.Empty;
    
    [JsonPropertyName("lang")]
    public string Lang { get; set; } = "vi";
    
    [JsonPropertyName("signature")]
    public string Signature { get; set; } = string.Empty;
}

/// <summary>
/// Response from MoMo after creating payment
/// </summary>
public class MoMoPaymentResponse
{
    [JsonPropertyName("partnerCode")]
    public string PartnerCode { get; set; } = string.Empty;
    
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;
    
    [JsonPropertyName("orderId")]
    public string OrderId { get; set; } = string.Empty;
    
    [JsonPropertyName("amount")]
    public long Amount { get; set; }
    
    [JsonPropertyName("responseTime")]
    public long ResponseTime { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("resultCode")]
    public int ResultCode { get; set; }
    
    [JsonPropertyName("payUrl")]
    public string PayUrl { get; set; } = string.Empty;
    
    [JsonPropertyName("deeplink")]
    public string Deeplink { get; set; } = string.Empty;
    
    [JsonPropertyName("qrCodeUrl")]
    public string QrCodeUrl { get; set; } = string.Empty;
    
    public bool IsSuccess => ResultCode == 0;
}

/// <summary>
/// IPN (Instant Payment Notification) from MoMo
/// </summary>
public class MoMoIpnRequest
{
    [JsonPropertyName("partnerCode")]
    public string PartnerCode { get; set; } = string.Empty;
    
    [JsonPropertyName("orderId")]
    public string OrderId { get; set; } = string.Empty;
    
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;
    
    [JsonPropertyName("amount")]
    public long Amount { get; set; }
    
    [JsonPropertyName("orderInfo")]
    public string OrderInfo { get; set; } = string.Empty;
    
    [JsonPropertyName("orderType")]
    public string OrderType { get; set; } = string.Empty;
    
    [JsonPropertyName("transId")]
    public long TransId { get; set; }
    
    [JsonPropertyName("resultCode")]
    public int ResultCode { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("payType")]
    public string PayType { get; set; } = string.Empty;
    
    [JsonPropertyName("responseTime")]
    public long ResponseTime { get; set; }
    
    [JsonPropertyName("extraData")]
    public string ExtraData { get; set; } = string.Empty;
    
    [JsonPropertyName("signature")]
    public string Signature { get; set; } = string.Empty;
    
    public bool IsSuccess => ResultCode == 0;
}

/// <summary>
/// Query parameters when user returns from MoMo
/// </summary>
public class MoMoReturnRequest
{
    public string PartnerCode { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string OrderInfo { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public long TransId { get; set; }
    public int ResultCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string PayType { get; set; } = string.Empty;
    public long ResponseTime { get; set; }
    public string ExtraData { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
    
    public bool IsSuccess => ResultCode == 0;
}
