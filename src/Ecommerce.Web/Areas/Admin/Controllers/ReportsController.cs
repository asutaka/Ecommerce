using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ReportsController(EcommerceDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30).Date;
        
        // Get completed orders from last 30 days
        var completedOrders = await dbContext.Orders
            .Include(x => x.Items)
            .Where(x => x.Status == OrderStatus.Completed && x.CreatedAt >= thirtyDaysAgo)
            .AsNoTracking()
            .ToListAsync();

        // Calculate summary statistics
        var totalRevenue = completedOrders.Sum(x => x.Total);
        var totalOrders = completedOrders.Count;
        var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

        // Daily revenue for last 30 days
        var dailyRevenue = Enumerable.Range(0, 30)
            .Select(i => thirtyDaysAgo.AddDays(i))
            .Select(date => new DailyRevenueViewModel
            {
                Date = date,
                Revenue = completedOrders
                    .Where(o => o.CreatedAt.Date == date)
                    .Sum(o => o.Total)
            })
            .ToList();

        // Best-selling products (top 10 by quantity)
        var bestSellingProducts = completedOrders
            .SelectMany(o => o.Items)
            .GroupBy(item => item.ProductName)
            .Select(g => new BestSellingProductViewModel
            {
                ProductName = g.Key,
                QuantitySold = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.UnitPrice * x.Quantity)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(10)
            .ToList();

        var model = new ReportDashboardViewModel
        {
            TotalRevenue = totalRevenue,
            TotalOrders = totalOrders,
            AverageOrderValue = averageOrderValue,
            DailyRevenue = dailyRevenue,
            BestSellingProducts = bestSellingProducts
        };

        return View(model);
    }
}
