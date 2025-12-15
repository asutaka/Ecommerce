namespace Ecommerce.Web.Areas.Admin.ViewModels;

public class PermissionsViewModel
{
    public Guid GroupId { get; set; }
    public string GroupCode { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    
    // Module permissions - key: module name, value: permission flags
    public Dictionary<string, int> Permissions { get; set; } = new();
    
    // Static list of all available modules
    public static List<ModuleInfo> AllModules => new()
    {
        new ModuleInfo { Key = "Dashboard", Name = "Dashboard" },
        new ModuleInfo { Key = "Products", Name = "Sản phẩm" },
        new ModuleInfo { Key = "Categories", Name = "Danh mục" },
        new ModuleInfo { Key = "Orders", Name = "Đơn hàng" },
        new ModuleInfo { Key = "Reports", Name = "Báo cáo" },
        new ModuleInfo { Key = "Cart", Name = "Giỏ hàng" },
        new ModuleInfo { Key = "Settings", Name = "Cấu hình" },
        new ModuleInfo { Key = "Warehouses", Name = "Thông tin kho hàng" },
        new ModuleInfo { Key = "Suppliers", Name = "Nhà cung cấp" },
        new ModuleInfo { Key = "Coupons", Name = "Mã khuyến mãi" },
        new ModuleInfo { Key = "Groups", Name = "Nhóm tài khoản" },
        new ModuleInfo { Key = "AdminUsers", Name = "Tài khoản Admin" },
        new ModuleInfo { Key = "Customers", Name = "Tài khoản khách hàng" }
    };
}

public class ModuleInfo
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
