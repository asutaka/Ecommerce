using Ecommerce.Web.ViewModels;

namespace Ecommerce.Web.Services;

public interface ICartService
{
    /// <summary>
    /// Get existing cart or create a new one for the session
    /// </summary>
    Task<Guid> GetOrCreateCartAsync(string sessionId);

    /// <summary>
    /// Add a product to the cart (or update quantity if already exists)
    /// </summary>
    Task<bool> AddItemAsync(Guid cartId, Guid productId, int quantity = 1, Guid? variantId = null);

    /// <summary>
    /// Update the quantity of a cart item
    /// </summary>
    Task<bool> UpdateItemQuantityAsync(Guid cartItemId, int quantity);

    /// <summary>
    /// Remove an item from the cart
    /// </summary>
    Task<bool> RemoveItemAsync(Guid cartItemId);

    /// <summary>
    /// Clear all items from the cart
    /// </summary>
    Task<bool> ClearCartAsync(Guid cartId);

    /// <summary>
    /// Get cart summary with all items and totals
    /// </summary>
    Task<CartViewModel?> GetCartSummaryAsync(Guid cartId);

    /// <summary>
    /// Get cart item count for badge display
    /// </summary>
    Task<int> GetCartItemCountAsync(Guid cartId);

    /// <summary>
    /// Merge session cart with user cart upon login
    /// </summary>
    Task MergeCartAsync(string sessionId, Guid customerId);

    /// <summary>
    /// Get cart ID by customer ID (for logged-in users)
    /// </summary>
    Task<Guid?> GetCartByCustomerIdAsync(Guid customerId);

    /// <summary>
    /// Apply coupon code to cart
    /// </summary>
    Task<(bool success, string message, decimal discount)> ApplyCouponAsync(Guid cartId, string couponCode);

    /// <summary>
    /// Remove applied coupon from cart
    /// </summary>
    Task<bool> RemoveCouponAsync(Guid cartId);
    
    /// <summary>
    /// Get available coupons for display
    /// </summary>
    Task<List<CouponViewModel>> GetAvailableCouponsAsync();
}
