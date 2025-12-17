using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Services;

public class CartService(EcommerceDbContext dbContext, ILogger<CartService> logger) : ICartService
{
    public async Task<Guid> GetOrCreateCartAsync(string sessionId)
    {
        var cart = await dbContext.ShoppingCarts
            .FirstOrDefaultAsync(x => x.SessionId == sessionId);

        if (cart == null)
        {
            cart = new ShoppingCart
            {
                SessionId = sessionId
            };
            dbContext.ShoppingCarts.Add(cart);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Created new cart {CartId} for session {SessionId}", cart.Id, sessionId);
        }

        return cart.Id;
    }

    public async Task<bool> AddItemAsync(Guid cartId, Guid productId, int quantity = 1, Guid? variantId = null)
    {
        try
        {
            if (quantity <= 0)
            {
                logger.LogWarning("Invalid quantity {Quantity} for product {ProductId}", quantity, productId);
                return false;
            }

            var product = await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == productId);
            if (product == null)
            {
                logger.LogWarning("Product {ProductId} not found", productId);
                return false;
            }

            // Load variant if specified
            ProductVariant? variant = null;
            if (variantId.HasValue)
            {
                variant = await dbContext.ProductVariants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == variantId.Value);
                if (variant == null)
                {
                    logger.LogWarning("Product variant {VariantId} not found", variantId.Value);
                    return false;
                }
            }

            // Check if cart exists
            var cartExists = await dbContext.ShoppingCarts.AnyAsync(x => x.Id == cartId);
            if (!cartExists)
            {
                logger.LogWarning("Cart {CartId} not found", cartId);
                return false;
            }

            // Check if item already exists in cart
            var existingItem = await dbContext.CartItems
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CartId == cartId && x.ProductId == productId && x.ProductVariantId == variantId);

            if (existingItem != null)
            {
                // Update quantity using ExecuteUpdate to avoid tracking issues
                await dbContext.CartItems
                    .Where(x => x.Id == existingItem.Id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.Quantity, existingItem.Quantity + quantity)
                        .SetProperty(x => x.UpdatedAt, DateTime.UtcNow));
                
                logger.LogInformation("Updated quantity for product {ProductId} in cart {CartId} to {Quantity}", 
                    productId, cartId, existingItem.Quantity + quantity);
            }
            else
            {
                // Add new item with product snapshot
                var cartItem = new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    ProductVariantId = variantId,
                    ProductName = product.Name,
                    ProductImageUrl = product.HeroImageUrl ?? "https://placehold.co/600x400?text=No+Image",
                    UnitPrice = variant?.Price ?? product.Price,
                    OriginalPrice = variant?.OriginalPrice ?? product.OriginalPrice,
                    Quantity = quantity,
                    VariantSKU = variant?.SKU,
                    VariantColor = variant?.Color,
                    VariantSize = variant?.Size
                };
                
                dbContext.CartItems.Add(cartItem);
                await dbContext.SaveChangesAsync();
                
                logger.LogInformation("Added product {ProductId} (variant: {VariantId}) to cart {CartId} with quantity {Quantity}", 
                    productId, variantId, cartId, quantity);
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding product {ProductId} to cart {CartId}", productId, cartId);
            return false;
        }
    }

    public async Task<bool> UpdateItemQuantityAsync(Guid cartItemId, int quantity)
    {
        var cartItem = await dbContext.CartItems.FindAsync(cartItemId);
        if (cartItem == null)
        {
            logger.LogWarning("Cart item {CartItemId} not found", cartItemId);
            return false;
        }

        if (quantity <= 0)
        {
            // Remove item if quantity is 0 or negative
            dbContext.CartItems.Remove(cartItem);
            logger.LogInformation("Removed cart item {CartItemId} due to zero/negative quantity", cartItemId);
        }
        else
        {
            cartItem.Quantity = quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
            logger.LogInformation("Updated cart item {CartItemId} quantity to {Quantity}", cartItemId, quantity);
        }

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveItemAsync(Guid cartItemId)
    {
        var cartItem = await dbContext.CartItems.FindAsync(cartItemId);
        if (cartItem == null)
        {
            logger.LogWarning("Cart item {CartItemId} not found", cartItemId);
            return false;
        }

        dbContext.CartItems.Remove(cartItem);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Removed cart item {CartItemId}", cartItemId);
        return true;
    }

    public async Task<bool> ClearCartAsync(Guid cartId)
    {
        var cart = await dbContext.ShoppingCarts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == cartId);

        if (cart == null)
        {
            logger.LogWarning("Cart {CartId} not found", cartId);
            return false;
        }

        dbContext.CartItems.RemoveRange(cart.Items);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Cleared all items from cart {CartId}", cartId);
        return true;
    }

    public async Task<CartViewModel?> GetCartSummaryAsync(Guid cartId)
    {
        var cart = await dbContext.ShoppingCarts
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == cartId);

        if (cart == null)
        {
            return null;
        }

        return new CartViewModel
        {
            CartId = cart.Id,
            AppliedCouponCode = cart.AppliedCouponCode,
            Discount = cart.Discount,
            Items = cart.Items.Select(item => new CartItemViewModel
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductImageUrl = item.ProductImageUrl,
                VariantColor = item.VariantColor,
                VariantSize = item.VariantSize,
                UnitPrice = item.UnitPrice,
                OriginalPrice = item.OriginalPrice,
                Quantity = item.Quantity
            }).ToList()
        };
    }

    public async Task<int> GetCartItemCountAsync(Guid cartId)
    {
        var cart = await dbContext.ShoppingCarts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == cartId);

        return cart?.Items.Sum(x => x.Quantity) ?? 0;
    }

    public async Task MergeCartAsync(string sessionId, Guid customerId)
    {
        // 1. Get the session cart (guest cart)
        var sessionCart = await dbContext.ShoppingCarts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.SessionId == sessionId && x.CustomerId == null);

        // 2. Get the user's existing cart (if any)
        var userCart = await dbContext.ShoppingCarts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.CustomerId == customerId);

        // Case 1: No session cart to merge
        if (sessionCart == null)
        {
            if (userCart != null)
            {
                // Just ensure the user cart is associated with current session
                userCart.SessionId = sessionId;
                await dbContext.SaveChangesAsync();
            }
            return;
        }

        // Case 2: Session cart exists, but no user cart
        if (userCart == null)
        {
            // Simply assign the session cart to the user
            sessionCart.CustomerId = customerId;
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Assigned guest cart {CartId} to customer {CustomerId}", sessionCart.Id, customerId);
            return;
        }

        // Case 3: Both exist - Merge items from session cart to user cart
        foreach (var sessionItem in sessionCart.Items)
        {
            var existingUserItem = userCart.Items
                .FirstOrDefault(x => x.ProductId == sessionItem.ProductId && x.ProductVariantId == sessionItem.ProductVariantId);

            if (existingUserItem != null)
            {
                // Update quantity
                existingUserItem.Quantity += sessionItem.Quantity;
                existingUserItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Move item to user cart
                // We need to create a new instance or reassign the foreign key. 
                // Reassigning FK is safer to avoid tracking issues with EF Core if we were to just add the object.
                // But since we are going to delete the session cart, let's create new items for the user cart.
                var newItem = new CartItem
                {
                    CartId = userCart.Id,
                    ProductId = sessionItem.ProductId,
                    ProductVariantId = sessionItem.ProductVariantId,
                    ProductName = sessionItem.ProductName,
                    ProductImageUrl = sessionItem.ProductImageUrl,
                    UnitPrice = sessionItem.UnitPrice,
                    OriginalPrice = sessionItem.OriginalPrice,
                    Quantity = sessionItem.Quantity,
                    VariantSKU = sessionItem.VariantSKU,
                    VariantColor = sessionItem.VariantColor,
                    VariantSize = sessionItem.VariantSize
                };
                userCart.Items.Add(newItem);
            }
        }

        // Update user cart session ID to current session
        userCart.SessionId = sessionId;
        userCart.UpdatedAt = DateTime.UtcNow;

        // Remove the old session cart
        dbContext.ShoppingCarts.Remove(sessionCart);

        await dbContext.SaveChangesAsync();
        logger.LogInformation("Merged guest cart {GuestCartId} into user cart {UserCartId} for customer {CustomerId}", 
            sessionCart.Id, userCart.Id, customerId);
    }

    public async Task<(bool success, string message, decimal discount)> ApplyCouponAsync(Guid cartId, string couponCode)
    {
        try
        {
            // Get cart with items
            var cart = await dbContext.ShoppingCarts
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == cartId);

            if (cart == null)
            {
                return (false, "Giỏ hàng không tồn tại", 0);
            }

            if (!cart.Items.Any())
            {
                return (false, "Giỏ hàng trống", 0);
            }

            // Find coupon
            var coupon = await dbContext.Coupons
                .FirstOrDefaultAsync(x => x.Code == couponCode.ToUpper());

            if (coupon == null)
            {
                return (false, "Mã giảm giá không tồn tại", 0);
            }

            // Validate coupon
            if (!coupon.IsActive)
            {
                return (false, "Mã giảm giá đã bị vô hiệu hóa", 0);
            }

            if (DateTime.UtcNow < coupon.StartDate)
            {
                return (false, "Mã giảm giá chưa có hiệu lực", 0);
            }

            if (DateTime.UtcNow > coupon.EndDate)
            {
                return (false, "Mã giảm giá đã hết hạn", 0);
            }

            if (coupon.UsageLimit > 0 && coupon.UsedCount >= coupon.UsageLimit)
            {
                return (false, "Mã giảm giá đã hết lượt sử dụng", 0);
            }

            // Calculate subtotal
            var subtotal = cart.Items.Sum(x => x.UnitPrice * x.Quantity);

            if (subtotal < coupon.MinimumOrderAmount)
            {
                return (false, $"Đơn hàng tối thiểu {coupon.MinimumOrderAmount:N0}đ để sử dụng mã này", 0);
            }

            // Calculate discount
            decimal discount = 0;
            if (coupon.DiscountPercentage.HasValue)
            {
                discount = subtotal * coupon.DiscountPercentage.Value / 100;
            }
            else
            {
                discount = coupon.DiscountAmount;
            }

            // Apply discount to cart
            cart.AppliedCouponCode = coupon.Code;
            cart.Discount = discount;
            cart.UpdatedAt = DateTime.UtcNow;

            // Increment usage count
            coupon.UsedCount++;
            coupon.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Applied coupon {CouponCode} to cart {CartId}. Discount: {Discount}", 
                coupon.Code, cartId, discount);

            return (true, $"Đã áp dụng mã giảm giá. Giảm {discount:N0}đ", discount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error applying coupon {CouponCode} to cart {CartId}", couponCode, cartId);
            return (false, "Có lỗi xảy ra khi áp dụng mã giảm giá", 0);
        }
    }

    public async Task<bool> RemoveCouponAsync(Guid cartId)
    {
        try
        {
            var cart = await dbContext.ShoppingCarts.FirstOrDefaultAsync(x => x.Id == cartId);
            if (cart == null)
            {
                return false;
            }

            // Decrement usage count if coupon was applied
            if (!string.IsNullOrEmpty(cart.AppliedCouponCode))
            {
                var coupon = await dbContext.Coupons
                    .FirstOrDefaultAsync(x => x.Code == cart.AppliedCouponCode);
                
                if (coupon != null && coupon.UsedCount > 0)
                {
                    coupon.UsedCount--;
                    coupon.UpdatedAt = DateTime.UtcNow;
                }
            }

            cart.AppliedCouponCode = null;
            cart.Discount = 0;
            cart.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Removed coupon from cart {CartId}", cartId);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing coupon from cart {CartId}", cartId);
            return false;
        }
    }
}
