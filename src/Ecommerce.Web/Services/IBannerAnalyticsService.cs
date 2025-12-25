using Ecommerce.Infrastructure.Entities;

namespace Ecommerce.Web.Services;

public interface IBannerAnalyticsService
{
    /// <summary>
    /// Track a banner view (impression)
    /// </summary>
    Task TrackViewAsync(Guid bannerId);
    
    /// <summary>
    /// Track a banner click
    /// </summary>
    Task TrackClickAsync(Guid bannerId);
    
    /// <summary>
    /// Get analytics summary for a specific banner within date range
    /// </summary>
    Task<BannerAnalyticsSummary> GetAnalyticsAsync(Guid bannerId, DateTime from, DateTime to);
    
    /// <summary>
    /// Get top performing banners by clicks
    /// </summary>
    Task<List<BannerPerformance>> GetTopPerformingBannersAsync(int count, DateTime from, DateTime to);
}

/// <summary>
/// Summary of analytics for a banner
/// </summary>
public class BannerAnalyticsSummary
{
    public Guid BannerId { get; set; }
    public string BannerTitle { get; set; } = string.Empty;
    public int TotalViews { get; set; }
    public int TotalClicks { get; set; }
    public double AverageCTR { get; set; }
    public List<DailyAnalytics> DailyBreakdown { get; set; } = new();
}

/// <summary>
/// Daily analytics data point
/// </summary>
public class DailyAnalytics
{
    public DateTime Date { get; set; }
    public int Views { get; set; }
    public int Clicks { get; set; }
    public double CTR { get; set; }
}

/// <summary>
/// Banner performance summary
/// </summary>
public class BannerPerformance
{
    public Guid BannerId { get; set; }
    public string BannerTitle { get; set; } = string.Empty;
    public int TotalClicks { get; set; }
    public int TotalViews { get; set; }
    public double CTR { get; set; }
}
