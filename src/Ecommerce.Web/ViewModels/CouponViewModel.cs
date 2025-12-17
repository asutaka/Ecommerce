namespace Ecommerce.Web.ViewModels;

public class CouponViewModel
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public DateTime EndDate { get; set; }
    public int RemainingUsage { get; set; }
    
    public string FormattedDiscount => DiscountPercentage.HasValue 
        ? $"Giảm {DiscountPercentage}%" 
        : $"Giảm {DiscountAmount:N0}đ";
    
    public string FormattedMinOrder => $"cho đơn từ {MinimumOrderAmount:N0}đ";
}
