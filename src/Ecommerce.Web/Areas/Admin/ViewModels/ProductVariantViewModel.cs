using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Web.Areas.Admin.ViewModels;

public class ProductVariantFormViewModel
{
    public Guid? Id { get; set; }
    public Guid ProductId { get; set; }
    
    [Required(ErrorMessage = "Vui lòng nhập SKU")]
    [MaxLength(50, ErrorMessage = "SKU không được quá 50 ký tự")]
    [Display(Name = "SKU")]
    public string SKU { get; set; } = string.Empty;
    
    [MaxLength(50, ErrorMessage = "Màu sắc không được quá 50 ký tự")]
    [Display(Name = "Màu sắc")]
    public string? Color { get; set; }
    
    [MaxLength(20, ErrorMessage = "Kích thước không được quá 20 ký tự")]
    [Display(Name = "Kích thước")]
    public string? Size { get; set; }
    
    [Required(ErrorMessage = "Vui lòng nhập số lượng")]
    [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải >= 0")]
    [Display(Name = "Số lượng")]
    public int Stock { get; set; } = 0;
    
    [Display(Name = "Giá riêng (tùy chọn)")]
    public decimal? Price { get; set; }
    
    [Display(Name = "Giá gốc (tùy chọn)")]
    public decimal? OriginalPrice { get; set; }
    
    [Display(Name = "Danh sách ảnh")]
    public List<string> ImageUrls { get; set; } = new();
    
    [Display(Name = "Hoạt động")]
    public bool IsActive { get; set; } = true;
}

public class ProductVariantListViewModel
{
    public Guid Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string? Size { get; set; }
    public int Stock { get; set; }
    public decimal? Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public bool IsActive { get; set; }
}
