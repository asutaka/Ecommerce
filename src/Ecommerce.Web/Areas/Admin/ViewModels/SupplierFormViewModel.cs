using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Web.Areas.Admin.ViewModels;

public class SupplierFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã nhà cung cấp")]
    [MaxLength(50, ErrorMessage = "Mã không được quá 50 ký tự")]
    [Display(Name = "Mã nhà cung cấp")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên nhà cung cấp")]
    [MaxLength(200, ErrorMessage = "Tên không được quá 200 ký tự")]
    [Display(Name = "Tên nhà cung cấp")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
    [MaxLength(500, ErrorMessage = "Địa chỉ không được quá 500 ký tự")]
    [Display(Name = "Địa chỉ")]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "Số điện thoại (Tối đa 5)")]
    public List<string> PhoneNumbers { get; set; } = new List<string> { "", "", "", "", "" };
}
