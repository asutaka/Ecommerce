using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Services;

public interface IOrderService
{
    Task<Order> CreateOrderFromCartAsync(Guid cartId, CartViewModel checkoutInfo);
    Task<Order?> GetOrderByIdAsync(Guid orderId);
}

public class OrderService(EcommerceDbContext dbContext, ILogger<OrderService> logger) : IOrderService
{
    public async Task<Order> CreateOrderFromCartAsync(Guid cartId, CartViewModel checkoutInfo)
    {
        var cart = await dbContext.ShoppingCarts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == cartId);

        if (cart == null || !cart.Items.Any())
        {
            throw new InvalidOperationException("Cart is empty or not found");
        }

        // Create order
        var order = new Order
        {
            CustomerName = checkoutInfo.CustomerName ?? "Guest",
            CustomerEmail = checkoutInfo.CustomerEmail ?? "",
            CustomerPhone = checkoutInfo.CustomerPhone,
            ShippingAddress = checkoutInfo.ShippingAddress,
            Note = checkoutInfo.Note,
            PaymentMethod = checkoutInfo.PaymentMethod,
            ShippingFee = checkoutInfo.ShippingFee,
            Discount = checkoutInfo.Discount,
            CouponCode = checkoutInfo.AppliedCouponCode,
            Subtotal = checkoutInfo.Subtotal,
            Total = checkoutInfo.Total,
            Status = OrderStatus.PendingInvoice,
            CustomerId = cart.CustomerId
        };

        // Add order items
        foreach (var cartItem in cart.Items)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = cartItem.ProductId,
                ProductVariantId = cartItem.ProductVariantId,
                ProductName = cartItem.ProductName,
                VariantSKU = cartItem.VariantSKU,
                VariantColor = cartItem.VariantColor,
                VariantSize = cartItem.VariantSize,
                UnitPrice = cartItem.UnitPrice,
                Quantity = cartItem.Quantity
            });
        }

        dbContext.Orders.Add(order);
        
        // Clear cart
        dbContext.CartItems.RemoveRange(cart.Items);
        
        // Reset coupon if applied
        if (!string.IsNullOrEmpty(cart.AppliedCouponCode))
        {
            var coupon = await dbContext.Coupons
                .FirstOrDefaultAsync(x => x.Code == cart.AppliedCouponCode);
            if (coupon != null && coupon.UsedCount > 0)
            {
                coupon.UsedCount--;
            }
            cart.AppliedCouponCode = null;
            cart.Discount = 0;
        }

        await dbContext.SaveChangesAsync();
        
        logger.LogInformation("Created order {OrderId} from cart {CartId}", order.Id, cartId);
        
        return order;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid orderId)
    {
        return await dbContext.Orders
            .Include(x => x.Items)
            .Where(x => !x.IsDeleted) // Filter out soft-deleted orders
            .FirstOrDefaultAsync(x => x.Id == orderId);
    }
}
