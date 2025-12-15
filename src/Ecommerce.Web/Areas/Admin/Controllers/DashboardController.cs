using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminAuth")]
public class DashboardController(EcommerceDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        var orders = await dbContext.Orders
            .Include(x => x.Payment)
            .Include(x => x.Notification)
            .OrderByDescending(x => x.CreatedAt)
            .Take(8)
            .ToListAsync();

        var stats = new DashboardStats
        {
            Orders = await dbContext.Orders.CountAsync(),
            PaidOrders = await dbContext.Orders.CountAsync(x => x.Payment != null),
            Revenue = await dbContext.Payments.SumAsync(x => (decimal?)x.Amount) ?? 0,
            Notifications = await dbContext.Notifications.CountAsync()
        };

        var viewModel = new DashboardViewModel
        {
            Stats = stats,
            RecentOrders = orders.Select(order => new AdminOrderRow
            {
                OrderId = order.Id,
                Customer = order.CustomerName,
                Status = order.Status.ToString(),
                Total = order.Total,
                CreatedAt = order.CreatedAt
            }).ToList()
        };

        return View(viewModel);
    }
}
