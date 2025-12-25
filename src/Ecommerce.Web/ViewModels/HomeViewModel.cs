using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Web.ViewModels;

public class HomeViewModel
{
    public List<ProductViewModel> FeaturedProducts { get; init; } = [];
    public CheckoutViewModel Checkout { get; init; } = new();
    public List<CategorySummaryViewModel> FeaturedCategories { get; init; } = [];
    public List<ProductViewModel> NewArrivals { get; init; } = [];
    public Dictionary<string, List<ProductViewModel>> NewArrivalsByCategory { get; set; } = new();
    public HomeStatistics Statistics { get; init; } = new();
}

public class ProductViewModel
{
    public Guid Id { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<string> Images { get; init; } = [];
    public string HeroImageUrl => Images.FirstOrDefault() ?? "https://placehold.co/600x400?text=No+Image";
    public decimal Price { get; init; }
    public decimal? OriginalPrice { get; init; }
    public bool IsFeatured { get; init; }
}

public class CheckoutViewModel
{
    [Required]
    [Display(Name = "Họ và tên")]
    public string CustomerName { get; set; } = string.Empty;

    [Required, EmailAddress]
    [Display(Name = "Email")]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required]
    public Guid ProductId { get; set; }

    [Range(1, 10)]
    public int Quantity { get; set; } = 1;
}

public class CategorySummaryViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int ProductCount { get; init; }
}

public class HomeStatistics
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalOrders { get; set; }
}

