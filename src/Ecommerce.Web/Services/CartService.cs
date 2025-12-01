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

    public async Task<bool> AddItemAsync(Guid cartId, Guid productId, int quantity = 1)
    {
        if (quantity <= 0)
        {
            logger.LogWarning("Invalid quantity {Quantity} for product {ProductId}", quantity, productId);
            return false;
        }

        var product = await dbContext.Products.FindAsync(productId);
        if (product == null)
        {
            logger.LogWarning("Product {ProductId} not found", productId);
            return false;
        }

        var cart = await dbContext.ShoppingCarts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == cartId);

        if (cart == null)
        {
            logger.LogWarning("Cart {CartId} not found", cartId);
            return false;
        }

        // Check if product already in cart
        var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem != null)
        {
            // Update quantity instead of adding duplicate
            existingItem.Quantity += quantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
            logger.LogInformation("Updated quantity for product {ProductId} in cart {CartId} to {Quantity}", 
                productId, cartId, existingItem.Quantity);
        }
        else
        {
            // Add new item with product snapshot
            var cartItem = new CartItem
            {
                CartId = cartId,
                ProductId = productId,
                ProductName = product.Name,
                ProductImageUrl = product.HeroImageUrl,
                UnitPrice = product.Price,
                Quantity = quantity
            };
            cart.Items.Add(cartItem);
            logger.LogInformation("Added product {ProductId} to cart {CartId} with quantity {Quantity}", 
                productId, cartId, quantity);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        return true;
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
            Items = cart.Items.Select(item => new CartItemViewModel
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductImageUrl = item.ProductImageUrl,
                UnitPrice = item.UnitPrice,
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
}
