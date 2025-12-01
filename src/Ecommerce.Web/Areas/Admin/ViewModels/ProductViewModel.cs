using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Web.Areas.Admin.ViewModels;

public class ProductFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
    [MaxLength(250)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mô tả")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập giá")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
    public decimal Price { get; set; }

    public bool IsFeatured { get; set; }

    [Display(Name = "Link ảnh sản phẩm (Tối đa 5)")]
    public List<string> ImageUrls { get; set; } = new List<string> { "", "", "", "", "" };
}
