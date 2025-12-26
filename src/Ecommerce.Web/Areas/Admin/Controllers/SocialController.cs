using Ecommerce.Web.Services.Social;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminAuth", Roles = "Admin")]
public class SocialController : Controller
{
    private readonly ISocialService _socialService;
    private readonly Ecommerce.Infrastructure.Persistence.EcommerceDbContext _db;

    public SocialController(
        ISocialService socialService,
        Ecommerce.Infrastructure.Persistence.EcommerceDbContext db)
    {
        _socialService = socialService;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetShareInfo(Guid id)
    {
        try
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound("Không tìm thấy sản phẩm");

            var platforms = _socialService.GetAvailablePlatforms();
            
            return Json(new
            {
                productId = product.Id,
                productName = product.Name,
                price = product.Price,
                // Default message template
                defaultMessage = $"🔥 {product.Name}\n💰 Giá: {product.Price:N0}đ\n\n{product.Description}\n\n👉 Mua ngay tại: https://shoppe.vn/sp/{product.Id}",
                platforms = platforms.Select(p => new 
                {
                    id = p.Id,
                    name = p.Name,
                    iconClass = p.IconClass,
                    colorClass = p.ColorClass,
                    isConfigured = p.IsConfigured
                })
            });
        }
        catch (Exception ex)
        {
             return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Publish([FromBody] PublishRequest request)
    {
        try
        {
            if (request == null || !request.PlatformIds.Any())
            {
                return BadRequest("Vui lòng chọn ít nhất một nền tảng.");
            }

            var results = await _socialService.PublishToPlatformsAsync(request.ProductId, request.PlatformIds, request.Message);

            return Json(new
            {
                success = true,
                results = results
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    public class PublishRequest
    {
        public Guid ProductId { get; set; }
        public List<string> PlatformIds { get; set; } = new();
        public string Message { get; set; } = "";
    }
}
