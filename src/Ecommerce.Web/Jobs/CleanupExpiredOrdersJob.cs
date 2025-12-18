using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Jobs;

public class CleanupExpiredOrdersJob
{
    private readonly EcommerceDbContext _context;
    private readonly ILogger<CleanupExpiredOrdersJob> _logger;

    public CleanupExpiredOrdersJob(
        EcommerceDbContext context,
        ILogger<CleanupExpiredOrdersJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Execute()
    {
        _logger.LogInformation("Starting cleanup of expired orders...");

        var now = DateTime.UtcNow;
        var expiredOrders = await _context.Orders
            .Where(o => o.ExpiresAt != null && o.ExpiresAt <= now)
            .ToListAsync();

        if (expiredOrders.Any())
        {
            _context.Orders.RemoveRange(expiredOrders);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Cleaned up {Count} expired orders. IDs: {OrderIds}",
                expiredOrders.Count,
                string.Join(", ", expiredOrders.Select(o => o.Id)));
        }
        else
        {
            _logger.LogInformation("No expired orders found.");
        }
    }
}
