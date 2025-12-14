namespace Ecommerce.Web.ViewModels;

public class CouponViewModel
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public DateTime EndDate { get; set; }
}
