using Ecommerce.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Web.Controllers;

public class CartController(
    ICartService cartService, 
    ICustomerAuthService customerAuthService,
    IOrderService orderService,
    ILogger<CartController> logger) : Controller
{

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

        // Auto-fill customer info if logged in
        var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (customerIdClaim != null && Guid.TryParse(customerIdClaim, out var customerId))
        {
            var customer = await customerAuthService.GetByIdAsync(customerId);
            if (customer != null)
            {
                cartViewModel.CustomerName = customer.FullName;
                cartViewModel.CustomerPhone = customer.Phone;
                cartViewModel.CustomerEmail = customer.Email;
                cartViewModel.ShippingAddress = customer.ShippingAddress1; // Default to address 1
                
                // Load available addresses
                if (!string.IsNullOrEmpty(customer.ShippingAddress1))
                    cartViewModel.AvailableAddresses.Add(customer.ShippingAddress1);
                if (!string.IsNullOrEmpty(customer.ShippingAddress2))
                    cartViewModel.AvailableAddresses.Add(customer.ShippingAddress2);
                if (!string.IsNullOrEmpty(customer.ShippingAddress3))
                    cartViewModel.AvailableAddresses.Add(customer.ShippingAddress3);
            }
        }
        
        // Load available coupons
        cartViewModel.AvailableCoupons = await cartService.GetAvailableCouponsAsync();

        return View(cartViewModel);
    }

    /// <summary>
    /// Add product to cart (AJAX)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddToCart(Guid productId, int quantity = 1, Guid? variantId = null)
    {
        logger.LogInformation("AddToCart called: ProductId={ProductId}, Quantity={Quantity}, VariantId={VariantId}", 
            productId, quantity, variantId);
            
        var cartId = await GetOrCreateCartIdAsync();
        logger.LogInformation("Cart ID: {CartId}", cartId);
        
        var success = await cartService.AddItemAsync(cartId, productId, quantity, variantId);

        if (!success)
        {
            logger.LogWarning("Failed to add product {ProductId} to cart {CartId}", productId, cartId);
            return Json(new { success = false, message = "Không thể thêm sản phẩm vào giỏ hàng" });
        }

        var itemCount = await cartService.GetCartItemCountAsync(cartId);
        logger.LogInformation("Successfully added product {ProductId} to cart {CartId}. Item count: {ItemCount}", 
            productId, cartId, itemCount);
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
    /// Apply coupon code to cart (AJAX)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(string couponCode)
    {
        if (string.IsNullOrWhiteSpace(couponCode))
        {
            return Json(new { success = false, message = "Vui lòng nhập mã giảm giá" });
        }

        var cartId = await GetOrCreateCartIdAsync();
        var (success, message, discount) = await cartService.ApplyCouponAsync(cartId, couponCode);

        if (!success)
        {
            return Json(new { success = false, message });
        }

        var cartViewModel = await cartService.GetCartSummaryAsync(cartId);
        return Json(new
        {
            success = true,
            message,
            discount,
            couponCode = couponCode.ToUpper(),
            subtotal = cartViewModel?.Subtotal ?? 0,
            total = cartViewModel?.Total ?? 0
        });
    }

    /// <summary>
    /// Remove coupon from cart (AJAX)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RemoveCoupon()
    {
        var cartId = await GetOrCreateCartIdAsync();
        var success = await cartService.RemoveCouponAsync(cartId);

        if (!success)
        {
            return Json(new { success = false, message = "Không thể xóa mã giảm giá" });
        }

        var cartViewModel = await cartService.GetCartSummaryAsync(cartId);
        return Json(new
        {
            success = true,
            message = "Đã xóa mã giảm giá",
            subtotal = cartViewModel?.Subtotal ?? 0,
            total = cartViewModel?.Total ?? 0
        });
    }

    /// <summary>
    /// Helper method to get or create cart ID
    /// Priority: 1. User's cart (by CustomerId), 2. Guest cart (by cookie session ID)
    /// </summary>
    private async Task<Guid> GetOrCreateCartIdAsync()
    {
        const string CookieKeySessionId = "_CartSessionId";
        
        // Priority 1: If user is logged in, find cart by CustomerId
        var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (customerIdClaim != null && Guid.TryParse(customerIdClaim, out var customerId))
        {
            var userCart = await cartService.GetCartByCustomerIdAsync(customerId);
            if (userCart != null)
            {
                logger.LogInformation("Found cart {CartId} for customer {CustomerId}", userCart.Value, customerId);
                return userCart.Value;
            }
        }
        
        // Priority 2: Try to get session ID from cookie (for guest users)
        var sessionId = HttpContext.Request.Cookies[CookieKeySessionId];

        if (string.IsNullOrEmpty(sessionId))
        {
            // Generate new session ID
            sessionId = Guid.NewGuid().ToString();
            
            // Save to cookie (expires in 30 days)
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            };
            HttpContext.Response.Cookies.Append(CookieKeySessionId, sessionId, cookieOptions);
            logger.LogInformation("Created new session ID {SessionId} and saved to cookie", sessionId);
        }

        return await cartService.GetOrCreateCartAsync(sessionId);
    }

    /// <summary>
    /// Process checkout and create order
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(ViewModels.CartViewModel model)
    {
        try
        {
            var cartId = await GetOrCreateCartIdAsync();
            var cart = await cartService.GetCartSummaryAsync(cartId);

            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Giỏ hàng trống";
                return RedirectToAction(nameof(Index));
            }

            // Update cart with checkout info
            cart.CustomerName = model.CustomerName;
            cart.CustomerEmail = model.CustomerEmail;
            cart.CustomerPhone = model.CustomerPhone;
            cart.ShippingAddress = model.ShippingAddress;
            cart.Note = model.Note;
            cart.PaymentMethod = model.PaymentMethod;

            // Create order
            var order = await orderService.CreateOrderFromCartAsync(cartId, cart);

            logger.LogInformation("Order {OrderId} created successfully", order.Id);

            // Redirect to confirmation page
            return RedirectToAction("Confirmation", new { orderId = order.Id });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating order");
            TempData["Error"] = "Có lỗi xảy ra khi đặt hàng. Vui lòng thử lại.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Order confirmation page
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Confirmation(Guid orderId)
    {
        var order = await orderService.GetOrderByIdAsync(orderId);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}
