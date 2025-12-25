using System.ComponentModel.DataAnnotations;
using Ecommerce.Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Web.Areas.Admin.ViewModels;

public class BannerViewModel
{
    public Guid? Id { get; set; }
    
    [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
    [Display(Name = "Tiêu đề")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }
    
    [Display(Name = "Ảnh Desktop")]
    public string? ImageUrl { get; set; }
    
    [Display(Name = "Ảnh Mobile")]
    public string? MobileImageUrl { get; set; }
    
    [Display(Name = "File ảnh Desktop")]
    public IFormFile? ImageFile { get; set; }
    
    [Display(Name = "File ảnh Mobile")]
    public IFormFile? MobileImageFile { get; set; }
    
    [Required(ErrorMessage = "Loại banner là bắt buộc")]
    [Display(Name = "Loại Banner")]
    public BannerType BannerType { get; set; }
    
    [Required(ErrorMessage = "Vị trí hiển thị là bắt buộc")]
    [Display(Name = "Vị trí hiển thị")]
    public BannerPosition Position { get; set; }
    
    [StringLength(1024, ErrorMessage = "URL không được vượt quá 1024 ký tự")]
    [Display(Name = "Link URL")]
    [Url(ErrorMessage = "URL không hợp lệ")]
    public string? LinkUrl { get; set; }
    
    [Display(Name = "Mở trong tab mới")]
    public bool OpenInNewTab { get; set; } = false;
    
    [StringLength(10000, ErrorMessage = "Mã quảng cáo không được vượt quá 10000 ký tự")]
    [Display(Name = "Mã quảng cáo (HTML/JavaScript)")]
    [DataType(DataType.MultilineText)]
    public string? ExternalAdCode { get; set; }
    
    [Required(ErrorMessage = "Thứ tự hiển thị là bắt buộc")]
    [Display(Name = "Thứ tự hiển thị")]
    [Range(0, 999, ErrorMessage = "Thứ tự phải từ 0 đến 999")]
    public int DisplayOrder { get; set; } = 0;
    
    [Display(Name = "Kích hoạt")]
    public bool IsActive { get; set; } = true;
    
    [Display(Name = "Ngày bắt đầu")]
    [DataType(DataType.DateTime)]
    public DateTime? StartDate { get; set; }
    
    [Display(Name = "Ngày kết thúc")]
    [DataType(DataType.DateTime)]
    public DateTime? EndDate { get; set; }
    
    [Display(Name = "Danh mục")]
    public Guid? CategoryId { get; set; }
    
    // For display purposes
    public string? CategoryName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Helper properties for dropdowns
    public List<SelectListItem>? BannerTypes { get; set; }
    public List<SelectListItem>? Positions { get; set; }
    public List<SelectListItem>? Categories { get; set; }
}
