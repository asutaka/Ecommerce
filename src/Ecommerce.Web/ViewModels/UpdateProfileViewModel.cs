using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Web.ViewModels;

public class UpdateProfileViewModel
{
    [Display(Name = "Họ và tên")]
    [StringLength(100)]
    public string? FullName { get; set; }

    [Display(Name = "Số điện thoại")]
    [Phone]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Display(Name = "Địa chỉ giao hàng 1")]
    [StringLength(500)]
    public string? ShippingAddress1 { get; set; }

    [Display(Name = "Địa chỉ giao hàng 2")]
    [StringLength(500)]
    public string? ShippingAddress2 { get; set; }

    [Display(Name = "Địa chỉ giao hàng 3")]
    [StringLength(500)]
    public string? ShippingAddress3 { get; set; }
}
