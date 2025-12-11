using Ecommerce.Contracts;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.ViewModels;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Controllers;

public class OrdersController(
    EcommerceDbContext dbContext,
    IPublishEndpoint publishEndpoint,
    ILogger<OrdersController> logger) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout([Bind(Prefix = "Checkout")] CheckoutViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["CheckoutStatus"] = "invalid";
            return RedirectToAction("Index", "Home");
        }

        var product = await dbContext.Products
            .Where(x => x.Id == model.ProductId)
            .Select(x => new { x.Id, x.Name, x.Price })
            .FirstOrDefaultAsync();

        if (product is null)
        {
            TempData["CheckoutStatus"] = "missing";
            return RedirectToAction("Index", "Home");
        }

        var orderId = Guid.NewGuid();
        await publishEndpoint.Publish(new CreateInvoice(
            orderId,
            model.CustomerName,
            model.CustomerEmail,
            new[]
            {
                new OrderLine(product.Id, product.Name, product.Price, model.Quantity)
            }));

        logger.LogInformation("Checkout dispatched for order {OrderId}", orderId);
        TempData["CheckoutStatus"] = "success";
        return RedirectToAction("Index", "Home");
    }
}

