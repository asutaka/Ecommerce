namespace Ecommerce.Web.Areas.Admin.ViewModels;

public class DashboardViewModel
{
    public required DashboardStats Stats { get; init; }
    public IReadOnlyCollection<AdminOrderRow> RecentOrders { get; init; } = Array.Empty<AdminOrderRow>();
}

public class DashboardStats
{
    public int Orders { get; init; }
    public int PaidOrders { get; init; }
    public decimal Revenue { get; init; }
    public int Notifications { get; init; }
}

public class AdminOrderRow
{
    public required Guid OrderId { get; init; }
    public required string Customer { get; init; }
    public required string Status { get; init; }
    public decimal Total { get; init; }
    public DateTime CreatedAt { get; init; }
}

