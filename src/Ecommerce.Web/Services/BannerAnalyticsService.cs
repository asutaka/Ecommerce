using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Services;

public class BannerAnalyticsService(EcommerceDbContext context) : IBannerAnalyticsService
{
    private readonly EcommerceDbContext _context = context;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task TrackViewAsync(Guid bannerId)
    {
        await TrackMetricAsync(bannerId, incrementViews: true);
    }

    public async Task TrackClickAsync(Guid bannerId)
    {
        await TrackMetricAsync(bannerId, incrementViews: false);
    }

    private async Task TrackMetricAsync(Guid bannerId, bool incrementViews)
    {
        var today = DateTime.UtcNow.Date;

        // Use semaphore to prevent race conditions
        await _semaphore.WaitAsync();
        try
        {
            // Find existing analytics record for today
            var analytics = await _context.BannerAnalytics
                .FirstOrDefaultAsync(a => a.BannerId == bannerId && a.Date == today);

            if (analytics == null)
            {
                // Create new record for today
                analytics = new BannerAnalytics
                {
                    BannerId = bannerId,
                    Date = today,
                    Views = incrementViews ? 1 : 0,
                    Clicks = incrementViews ? 0 : 1
                };
                _context.BannerAnalytics.Add(analytics);
            }
            else
            {
                // Update existing record
                if (incrementViews)
                {
                    analytics.Views++;
                }
                else
                {
                    analytics.Clicks++;
                }
                
                analytics.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<BannerAnalyticsSummary> GetAnalyticsAsync(Guid bannerId, DateTime from, DateTime to)
    {
        // Ensure dates are UTC and date-only
        from = DateTime.SpecifyKind(from.Date, DateTimeKind.Utc);
        to = DateTime.SpecifyKind(to.Date, DateTimeKind.Utc);

        var banner = await _context.Banners.FindAsync(bannerId);
        if (banner == null)
        {
            throw new ArgumentException($"Banner with ID {bannerId} not found");
        }

        var analytics = await _context.BannerAnalytics
            .Where(a => a.BannerId == bannerId && a.Date >= from && a.Date <= to)
            .OrderBy(a => a.Date)
            .ToListAsync();

        var totalViews = analytics.Sum(a => a.Views);
        var totalClicks = analytics.Sum(a => a.Clicks);
        var avgCTR = totalViews > 0 ? Math.Round((double)totalClicks / totalViews * 100, 2) : 0;

        return new BannerAnalyticsSummary
        {
            BannerId = bannerId,
            BannerTitle = banner.Title,
            TotalViews = totalViews,
            TotalClicks = totalClicks,
            AverageCTR = avgCTR,
            DailyBreakdown = analytics.Select(a => new DailyAnalytics
            {
                Date = a.Date,
                Views = a.Views,
                Clicks = a.Clicks,
                CTR = a.ClickThroughRate
            }).ToList()
        };
    }

    public async Task<List<BannerPerformance>> GetTopPerformingBannersAsync(int count, DateTime from, DateTime to)
    {
        // Ensure dates are UTC and date-only
        from = DateTime.SpecifyKind(from.Date, DateTimeKind.Utc);
        to = DateTime.SpecifyKind(to.Date, DateTimeKind.Utc);

        var topBanners = await _context.BannerAnalytics
            .Where(a => a.Date >= from && a.Date <= to)
            .GroupBy(a => new { a.BannerId, a.Banner.Title })
            .Select(g => new BannerPerformance
            {
                BannerId = g.Key.BannerId,
                BannerTitle = g.Key.Title,
                TotalClicks = g.Sum(a => a.Clicks),
                TotalViews = g.Sum(a => a.Views),
                CTR = g.Sum(a => a.Views) > 0 
                    ? Math.Round((double)g.Sum(a => a.Clicks) / g.Sum(a => a.Views) * 100, 2) 
                    : 0
            })
            .OrderByDescending(p => p.TotalClicks)
            .Take(count)
            .ToListAsync();

        return topBanners;
    }
}
