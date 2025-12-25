using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Infrastructure.Entities;

/// <summary>
/// Tracks daily analytics metrics for banners
/// </summary>
public class BannerAnalytics : BaseEntity
{
    /// <summary>
    /// Foreign key to Banner
    /// </summary>
    public Guid BannerId { get; set; }
    
    /// <summary>
    /// Navigation property to Banner
    /// </summary>
    public Banner Banner { get; set; } = null!;
    
    /// <summary>
    /// Date for this analytics record (UTC, date only - no time component)
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Number of times banner was viewed (impressions)
    /// </summary>
    public int Views { get; set; }
    
    /// <summary>
    /// Number of times banner was clicked
    /// </summary>
    public int Clicks { get; set; }
    
    /// <summary>
    /// Click-Through Rate (CTR) as percentage
    /// Computed property: (Clicks / Views) * 100
    /// </summary>
    [NotMapped]
    public double ClickThroughRate => Views > 0 ? Math.Round((double)Clicks / Views * 100, 2) : 0;
}
