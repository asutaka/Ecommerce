using Ecommerce.Infrastructure.Entities;
using Ecommerce.Web.Models;

namespace Ecommerce.Web.Areas.Admin.ViewModels;

public class CartListViewModel
{
    public PaginatedList<ShoppingCart> Carts { get; set; } = default!;
}

public class CartDetailsViewModel
{
    public Guid Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public DateTime? LastUpdated { get; set; }
    public List<CartItemViewModel> Items { get; set; } = new();
    public decimal TotalValue => Items.Sum(x => x.LineTotal);
}

public class CartItemViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
    public string ProductImageUrl { get; set; } = string.Empty;
}

public class CartAnalyticsViewModel
{
    public int TotalCarts { get; set; }
    public int ActiveCarts { get; set; }
    public int AbandonedCarts { get; set; }
    public double AbandonmentRate { get; set; }
    public List<TopAbandonedItemViewModel> TopAbandonedItems { get; set; } = new();
}

public class TopAbandonedItemViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public int AbandonedCount { get; set; }
}
