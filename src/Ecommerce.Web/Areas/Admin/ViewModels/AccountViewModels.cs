using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Web.Areas.Admin.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email hoặc Username là bắt buộc")]
    [Display(Name = "Email hoặc Username")]
    public string EmailOrUsername { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Ghi nhớ đăng nhập")]
    public bool RememberMe { get; set; }
}

public class ProfileViewModel
{
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(320)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "Họ tên")]
    public string? FullName { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu hiện tại")]
    public string? CurrentPassword { get; set; }

    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có từ 6-100 ký tự")]
    [Display(Name = "Mật khẩu mới")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [Display(Name = "Xác nhận mật khẩu mới")]
    public string? ConfirmNewPassword { get; set; }
}
