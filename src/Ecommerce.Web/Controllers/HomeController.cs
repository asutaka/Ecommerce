using System.Diagnostics;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Models;
using Ecommerce.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Controllers;

public class HomeController(
    ILogger<HomeController> logger,
    EcommerceDbContext dbContext) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var products = await dbContext.Products
            .OrderByDescending(x => x.IsFeatured)
            .ThenBy(x => x.Name)
            .Select(x => new ProductViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Images = x.Images,
                Price = x.Price,
                IsFeatured = x.IsFeatured
            })
            .ToListAsync();

        var model = new HomeViewModel
        {
            FeaturedProducts = products,
            Checkout = new CheckoutViewModel
            {
                ProductId = products.FirstOrDefault()?.Id ?? Guid.Empty
            }
        };

        logger.LogInformation("Rendering storefront with {ProductCount} sản phẩm", products.Count);

        ViewBag.CheckoutStatus = TempData["CheckoutStatus"]?.ToString();

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
