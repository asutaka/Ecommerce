using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminAuth")]
public class CartsController(EcommerceDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(int? pageNumber)
    {
        // Active carts: Updated within last 24 hours and has items
        var cutoff = DateTime.UtcNow.AddHours(-24);
        
        var query = dbContext.ShoppingCarts
            .Include(x => x.Items)
            .Where(x => x.UpdatedAt >= cutoff && x.Items.Any())
            .OrderByDescending(x => x.UpdatedAt);

        int pageSize = 10;
        var carts = await PaginatedList<ShoppingCart>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize);

        var model = new CartListViewModel
        {
            Carts = carts
        };

        return View(model);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var cart = await dbContext.ShoppingCarts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (cart == null)
        {
            return NotFound();
        }

        var model = new CartDetailsViewModel
        {
            Id = cart.Id,
            SessionId = cart.SessionId,
            CustomerId = cart.CustomerId,
            LastUpdated = cart.UpdatedAt,
            Items = cart.Items.Select(x => new CartItemViewModel
            {
                ProductName = x.ProductName,
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity,
                ProductImageUrl = x.ProductImageUrl
            }).ToList()
        };

        return View(model);
    }

    public async Task<IActionResult> Analytics()
    {
        var cutoff = DateTime.UtcNow.AddHours(-24);

        var allCarts = await dbContext.ShoppingCarts
            .Include(x => x.Items)
            .Where(x => x.Items.Any())
            .AsNoTracking()
            .ToListAsync();

        var totalCarts = allCarts.Count;
        var activeCarts = allCarts.Count(x => x.UpdatedAt >= cutoff);
        var abandonedCarts = allCarts.Count(x => x.UpdatedAt < cutoff);
        
        var abandonmentRate = totalCarts > 0 ? (double)abandonedCarts / totalCarts * 100 : 0;

        var topAbandonedItems = allCarts
            .Where(x => x.UpdatedAt < cutoff)
            .SelectMany(x => x.Items)
            .GroupBy(x => x.ProductName)
            .Select(g => new TopAbandonedItemViewModel
            {
                ProductName = g.Key,
                AbandonedCount = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.AbandonedCount)
            .Take(10)
            .ToList();

        var model = new CartAnalyticsViewModel
        {
            TotalCarts = totalCarts,
            ActiveCarts = activeCarts,
            AbandonedCarts = abandonedCarts,
            AbandonmentRate = Math.Round(abandonmentRate, 2),
            TopAbandonedItems = topAbandonedItems
        };

        return View(model);
    }
}
