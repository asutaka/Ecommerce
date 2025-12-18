using Ecommerce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentApiController : ControllerBase
{
    private readonly EcommerceDbContext _dbContext;
    private readonly ILogger<PaymentApiController> _logger;

    public PaymentApiController(
        EcommerceDbContext dbContext,
        ILogger<PaymentApiController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Get payment status for an order
    /// </summary>
    [HttpGet("status/{orderId}")]
    public async Task<IActionResult> GetPaymentStatus(Guid orderId)
    {
        try
        {
            var order = await _dbContext.Orders.FindAsync(orderId);
            
            if (order == null)
            {
                return NotFound(new { error = "Order not found" });
            }

            return Ok(new
            {
                status = order.Status.ToString(),
                attempts = order.PaymentAttempts,
                nextRetry = order.NextRetryScheduledAt,
                provider = order.PaymentProvider
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment status for order {OrderId}", orderId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}
