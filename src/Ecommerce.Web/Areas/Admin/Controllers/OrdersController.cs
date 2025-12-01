using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class OrdersController(EcommerceDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(int? pageNumber, OrderStatus? status)
    {
        var query = dbContext.Orders.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        int pageSize = 10;
        var orders = await PaginatedList<Order>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize);

        var model = new OrderListViewModel
        {
            Orders = orders,
            CurrentStatus = status
        };

        return View(model);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var order = await dbContext.Orders
            .Include(x => x.Items)
            .Include(x => x.Payment)
            .Include(x => x.Notification)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        var model = new OrderDetailsViewModel
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            Status = order.Status,
            Total = order.Total,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(x => new OrderItemViewModel
            {
                ProductName = x.ProductName,
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity
            }).ToList(),
            Payment = order.Payment != null ? new PaymentViewModel
            {
                Amount = order.Payment.Amount,
                Reference = order.Payment.Reference,
                ProcessedAt = order.Payment.ProcessedAt
            } : null,
            Notification = order.Notification != null ? new NotificationLogViewModel
            {
                Channel = order.Notification.Channel,
                Destination = order.Notification.Destination,
                SentAt = order.Notification.SentAt,
                Message = order.Notification.Message
            } : null
        };

        return View(model);
    }
}
