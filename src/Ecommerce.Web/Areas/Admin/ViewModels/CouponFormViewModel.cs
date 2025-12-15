using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Web.Areas.Admin.ViewModels;

public class CouponFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã khuyến mãi")]
    [MaxLength(50, ErrorMessage = "Mã không được quá 50 ký tự")]
    [Display(Name = "Mã khuyến mãi")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mô tả")]
    [MaxLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự")]
    [Display(Name = "Mô tả")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Giảm giá (VNĐ)")]
    [Range(0, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
    public decimal DiscountAmount { get; set; }

    [Display(Name = "Giảm giá (%)")]
    [Range(0, 100, ErrorMessage = "Phần trăm phải từ 0-100")]
    public decimal? DiscountPercentage { get; set; }

    [Display(Name = "Đơn hàng tối thiểu (VNĐ)")]
    [Range(0, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
    public decimal MinimumOrderAmount { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
    [Display(Name = "Ngày bắt đầu")]
    public DateTime StartDate { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
    [Display(Name = "Ngày kết thúc")]
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(30);

    [Display(Name = "Kích hoạt")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Giới hạn sử dụng")]
    [Range(0, int.MaxValue, ErrorMessage = "Số lần phải lớn hơn hoặc bằng 0")]
    public int UsageLimit { get; set; } = 100;
}
