namespace Ecommerce.Web.Models;

public class VNPayPaymentOptions
{
    public const string SectionName = "VNPayPayment";
    
    public string TmnCode { get; set; } = string.Empty;
    public string HashSecret { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public string Version { get; set; } = "2.1.0";
    public string Command { get; set; } = "pay";
    public string CurrCode { get; set; } = "VND";
    public string Locale { get; set; } = "vn";
    public bool UseMockService { get; set; } = true;
}

public class VNPayPaymentRequest
{
    public string Version { get; set; } = string.Empty;
    public string TmnCode { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public string CreateDate { get; set; } = string.Empty;
    public string CurrCode { get; set; } = string.Empty;
    public string IpAddr { get; set; } = string.Empty;
    public string Locale { get; set; } = string.Empty;
    public string OrderInfo { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public string TxnRef { get; set; } = string.Empty;
    public string SecureHash { get; set; } = string.Empty;
}

public class VNPayPaymentResponse
{
    public string PaymentUrl { get; set; } = string.Empty;
}

public class VNPayCallbackRequest
{
    public string TmnCode { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string BankTranNo { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public string OrderInfo { get; set; } = string.Empty;
    public string PayDate { get; set; } = string.Empty;
    public string ResponseCode { get; set; } = string.Empty;
    public string TransactionNo { get; set; } = string.Empty;
    public string TransactionStatus { get; set; } = string.Empty;
    public string TxnRef { get; set; } = string.Empty;
    public string SecureHash { get; set; } = string.Empty;
    
    public bool IsSuccess => ResponseCode == "00";
}
