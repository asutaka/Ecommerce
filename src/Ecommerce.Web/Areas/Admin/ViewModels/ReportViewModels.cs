namespace Ecommerce.Web.Areas.Admin.ViewModels;

public class ReportDashboardViewModel
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<DailyRevenueViewModel> DailyRevenue { get; set; } = new();
    public List<BestSellingProductViewModel> BestSellingProducts { get; set; } = new();
}

public class DailyRevenueViewModel
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
}

public class BestSellingProductViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}
