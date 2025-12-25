using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Web.Areas.Admin.ViewModels;

public class ProductFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
    [MaxLength(250)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "SEO Slug (tự động)")]
    public string? Slug { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mô tả")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập giá")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
    public decimal Price { get; set; }

    [Display(Name = "Giá gốc (tùy chọn)")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá gốc phải lớn hơn hoặc bằng 0")]
    public decimal? OriginalPrice { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsActive { get; set; } = true;

    [Required(ErrorMessage = "Vui lòng chọn danh mục")]
    [Display(Name = "Danh mục")]
    public Guid PrimaryCategoryId { get; set; }

    public List<SelectListItem> Categories { get; set; } = new();

    [Display(Name = "Link ảnh sản phẩm (Tối đa 5)")]
    public List<string> ImageUrls { get; set; } = new List<string> { "", "", "", "", "" };
}
