namespace Ecommerce.Web.Models;

public class ZaloPayPaymentOptions
{
    public const string SectionName = "ZaloPayPayment";
    
    public string AppId { get; set; } = string.Empty;
    public string Key1 { get; set; } = string.Empty;
    public string Key2 { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public bool UseMockService { get; set; } = true;
}

public class ZaloPayPaymentRequest
{
    public string AppId { get; set; } = string.Empty;
    public string AppUser { get; set; } = string.Empty;
    public long AppTime { get; set; }
    public long Amount { get; set; }
    public string AppTransId { get; set; } = string.Empty;
    public string EmbedData { get; set; } = "{}";
    public string Item { get; set; } = "[]";
    public string Description { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string Mac { get; set; } = string.Empty;
}

public class ZaloPayPaymentResponse
{
    public int ReturnCode { get; set; }
    public string ReturnMessage { get; set; } = string.Empty;
    public string OrderUrl { get; set; } = string.Empty;
    public string ZpTransToken { get; set; } = string.Empty;
}

public class ZaloPayCallbackRequest
{
    public string AppId { get; set; } = string.Empty;
    public string AppTransId { get; set; } = string.Empty;
    public long AppTime { get; set; }
    public long Amount { get; set; }
    public string EmbedData { get; set; } = string.Empty;
    public string Item { get; set; } = string.Empty;
    public long ZpTransId { get; set; }
    public string ServerTime { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string MerchantUserId { get; set; } = string.Empty;
    public long UserFeeAmount { get; set; }
    public long DiscountAmount { get; set; }
    public int Status { get; set; }
    public string Mac { get; set; } = string.Empty;
    
    public bool IsSuccess => Status == 1;
}
