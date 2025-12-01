using Ecommerce.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers;

public class CartController(ICartService cartService, ILogger<CartController> logger) : Controller
{
    private const string SessionKeyCartId = "_CartId";

    /// <summary>
    /// Display cart page
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cartId = await GetOrCreateCartIdAsync();
        var cartViewModel = await cartService.GetCartSummaryAsync(cartId);

        if (cartViewModel == null)
        {
            logger.LogWarning("Cart {CartId} not found", cartId);
            return View(new ViewModels.CartViewModel());
        }

        return View(cartViewModel);
    }

    /// <summary>
    /// Add product to cart (AJAX)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddToCart(Guid productId, int quantity = 1)
    {
        var cartId = await GetOrCreateCartIdAsync();
        var success = await cartService.AddItemAsync(cartId, productId, quantity);

        if (!success)
        {
            return Json(new { success = false, message = "Không thể thêm sản phẩm vào giỏ hàng" });
        }

        var itemCount = await cartService.GetCartItemCountAsync(cartId);
        return Json(new { success = true, message = "Đã thêm vào giỏ hàng", itemCount });
    }

    /// <summary>
    /// Update item quantity (AJAX)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(Guid cartItemId, int quantity)
    {
        var success = await cartService.UpdateItemQuantityAsync(cartItemId, quantity);

        if (!success)
        {
            return Json(new { success = false, message = "Không thể cập nhật số lượng" });
        }

        var cartId = await GetOrCreateCartIdAsync();
        var cartViewModel = await cartService.GetCartSummaryAsync(cartId);

        return Json(new
        {
            success = true,
            itemCount = cartViewModel?.ItemCount ?? 0,
            subtotal = cartViewModel?.Subtotal ?? 0,
            total = cartViewModel?.Total ?? 0
        });
    }

    /// <summary>
    /// Remove item from cart (AJAX)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RemoveItem(Guid cartItemId)
    {
        var success = await cartService.RemoveItemAsync(cartItemId);

        if (!success)
        {
            return Json(new { success = false, message = "Không thể xóa sản phẩm" });
        }

        var cartId = await GetOrCreateCartIdAsync();
        var cartViewModel = await cartService.GetCartSummaryAsync(cartId);

        return Json(new
        {
            success = true,
            message = "Đã xóa sản phẩm khỏi giỏ hàng",
            itemCount = cartViewModel?.ItemCount ?? 0,
            subtotal = cartViewModel?.Subtotal ?? 0,
            total = cartViewModel?.Total ?? 0
        });
    }

    /// <summary>
    /// Clear entire cart
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Clear()
    {
        var cartId = await GetOrCreateCartIdAsync();
        var success = await cartService.ClearCartAsync(cartId);

        if (!success)
        {
            TempData["ErrorMessage"] = "Không thể xóa giỏ hàng";
        }
        else
        {
            TempData["SuccessMessage"] = "Đã xóa tất cả sản phẩm khỏi giỏ hàng";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Get cart item count for badge (AJAX)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetItemCount()
    {
        var cartId = await GetOrCreateCartIdAsync();
        var itemCount = await cartService.GetCartItemCountAsync(cartId);
        return Json(new { itemCount });
    }

    /// <summary>
    /// Helper method to get or create cart ID from session
    /// </summary>
    private async Task<Guid> GetOrCreateCartIdAsync()
    {
        var sessionId = HttpContext.Session.GetString(SessionKeyCartId);

        if (string.IsNullOrEmpty(sessionId))
        {
            // Generate new session ID
            sessionId = Guid.NewGuid().ToString();
            HttpContext.Session.SetString(SessionKeyCartId, sessionId);
        }

        return await cartService.GetOrCreateCartAsync(sessionId);
    }
}
