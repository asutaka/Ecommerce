namespace Ecommerce.Infrastructure.Entities;

public enum BannerType
{
    Hero = 1,           // Banner carousel chính trang chủ
    TopBar = 2,         // Thông báo nổi bật trên đầu trang
    Promotional = 3,    // Banner khuyến mãi
    Category = 4,       // Banner cho category
    Popup = 5,          // Banner popup/modal
    LeftSidebar = 6,    // Banner sidebar bên trái
    RightSidebar = 7    // Banner sidebar bên phải
}

public enum BannerPosition
{
    All = 0,           // Hiển thị ở tất cả các trang
    Home = 1,          // Chỉ trang chủ
    CategoryPage = 2,  // Trang danh mục sản phẩm
    ProductDetail = 3  // Trang chi tiết sản phẩm
}

public class Banner : BaseEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    
    // Image URLs
    public required string ImageUrl { get; set; }          // Desktop image
    public string? MobileImageUrl { get; set; }            // Mobile image (optional)
    
    // Banner configuration
    public BannerType BannerType { get; set; }
    public BannerPosition Position { get; set; }
    
    // Link configuration
    public string? LinkUrl { get; set; }
    public bool OpenInNewTab { get; set; } = false;
    
    // External Ad Code (for Google AdSense, etc.)
    public string? ExternalAdCode { get; set; }  // HTML/JavaScript ad code
    
    // Display settings
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    
    // Date range
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Category relationship (for Category banners)
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    
    // Helper method to check if banner should be displayed
    public bool ShouldDisplay()
    {
        if (!IsActive) return false;
        
        var now = DateTime.UtcNow;
        
        if (StartDate.HasValue && now < StartDate.Value)
            return false;
            
        if (EndDate.HasValue && now > EndDate.Value)
            return false;
            
        return true;
    }
}
