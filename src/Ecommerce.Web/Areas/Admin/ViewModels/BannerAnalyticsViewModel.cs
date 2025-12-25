namespace Ecommerce.Web.Areas.Admin.ViewModels;

public class BannerAnalyticsViewModel
{
    public Guid BannerId { get; set; }
    public string BannerTitle { get; set; } = string.Empty;
    public string BannerType { get; set; } = string.Empty;
    
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    
    public int TotalViews { get; set; }
    public int TotalClicks { get; set; }
    public double AverageCTR { get; set; }
    
    public List<DailyAnalyticsData> DailyData { get; set; } = new();
}

public class DailyAnalyticsData
{
    public string Date { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Clicks { get; set; }
    public double CTR { get; set; }
}
