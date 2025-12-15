using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Web.Areas.Admin.ViewModels;

public class GroupFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã nhóm")]
    [StringLength(50, ErrorMessage = "Mã nhóm không được vượt quá 50 ký tự")]
    [Display(Name = "Mã nhóm")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên nhóm")]
    [StringLength(200, ErrorMessage = "Tên nhóm không được vượt quá 200 ký tự")]
    [Display(Name = "Tên nhóm")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Display(Name = "Trạng thái hoạt động")]
    public bool IsActive { get; set; } = true;
}
