using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Web.Areas.Admin.ViewModels;

public class AdminUserFormViewModel
{
    public Guid? Id { get; set; }
    
    [Required(ErrorMessage = "Username là bắt buộc")]
    [StringLength(50, ErrorMessage = "Username không được vượt quá 50 ký tự")]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(320, ErrorMessage = "Email không được vượt quá 320 ký tự")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
    
    [StringLength(200, ErrorMessage = "Họ tên không được vượt quá 200 ký tự")]
    [Display(Name = "Họ tên")]
    public string? FullName { get; set; }
    
    [Display(Name = "Nhóm")]
    public Guid? GroupId { get; set; }
    
    [Display(Name = "Kích hoạt")]
    public bool IsActive { get; set; } = true;
    
    // For create only
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có từ 6-100 ký tự")]
    [Display(Name = "Mật khẩu")]
    public string? Password { get; set; }
    
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string? ConfirmPassword { get; set; }
    
    // For dropdown
    public List<SelectListItem> Groups { get; set; } = new();
}

public class ResetPasswordViewModel
{
    public Guid AdminUserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có từ 6-100 ký tự")]
    [Display(Name = "Mật khẩu mới")]
    public string NewPassword { get; set; } = string.Empty;
    
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
