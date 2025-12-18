using Ecommerce.Infrastructure.Entities;

namespace Ecommerce.Infrastructure.Services;

/// <summary>
/// Policy for payment retry logic
/// </summary>
public static class PaymentRetryPolicy
{
    public const int MAX_AUTO_RETRIES = 3;
    public const int MAX_ATTEMPTS_PER_HOUR = 5;
    public const int MAX_TOTAL_ATTEMPTS = 10;
    public const int COOLDOWN_MINUTES_AFTER_3_FAILS = 5;
    public const int ORDER_EXPIRY_HOURS = 24;
    
    /// <summary>
    /// Check if payment can be retried
    /// </summary>
    public static bool CanRetryPayment(Order order, out string reason)
    {
        // Rule 1: Order already deleted (soft delete)
        if (order.IsDeleted)
        {
            reason = "Đơn hàng đã bị hủy";
            return false;
        }
        
        // Rule 2: Order expired
        if (order.ExpiresAt != null && order.ExpiresAt < DateTime.UtcNow)
        {
            reason = "Đơn hàng đã hết hạn";
            return false;
        }
        
        // Rule 3: Order already paid
        if (order.Status == OrderStatus.Paid)
        {
            reason = "Đơn hàng đã được thanh toán";
            return false;
        }
        
        // Rule 4: Too many attempts in last hour
        if (order.PaymentAttempts >= MAX_ATTEMPTS_PER_HOUR &&
            order.LastPaymentAttempt != null &&
            order.LastPaymentAttempt > DateTime.UtcNow.AddHours(-1))
        {
            reason = $"Bạn đã thử quá nhiều lần. Vui lòng đợi {GetCooldownMinutes(order)} phút";
            return false;
        }
        
        // Rule 5: Cooldown after 3 failed attempts
        if (order.PaymentAttempts >= 3 &&
            order.LastPaymentAttempt != null &&
            order.LastPaymentAttempt > DateTime.UtcNow.AddMinutes(-COOLDOWN_MINUTES_AFTER_3_FAILS))
        {
            var remaining = GetCooldownRemaining(order);
            reason = $"Vui lòng đợi {remaining?.TotalMinutes:F0} phút trước khi thử lại";
            return false;
        }
        
        // Rule 6: Max total attempts
        if (order.PaymentAttempts >= MAX_TOTAL_ATTEMPTS)
        {
            reason = "Đã vượt quá số lần thử tối đa. Vui lòng liên hệ hỗ trợ";
            return false;
        }
        
        reason = string.Empty;
        return true;
    }
    
    /// <summary>
    /// Get remaining cooldown time
    /// </summary>
    public static TimeSpan? GetCooldownRemaining(Order order)
    {
        if (order.LastPaymentAttempt == null || order.PaymentAttempts < 3)
            return null;
            
        var cooldownEnd = order.LastPaymentAttempt.Value.AddMinutes(COOLDOWN_MINUTES_AFTER_3_FAILS);
        var remaining = cooldownEnd - DateTime.UtcNow;
        
        return remaining > TimeSpan.Zero ? remaining : null;
    }
    
    /// <summary>
    /// Get cooldown minutes for display
    /// </summary>
    public static int GetCooldownMinutes(Order order)
    {
        var remaining = GetCooldownRemaining(order);
        return remaining.HasValue ? (int)Math.Ceiling(remaining.Value.TotalMinutes) : 0;
    }
    
    /// <summary>
    /// Calculate retry delay with exponential backoff
    /// </summary>
    public static TimeSpan CalculateRetryDelay(int attemptNumber)
    {
        return attemptNumber switch
        {
            1 => TimeSpan.FromMinutes(5),   // 5 minutes
            2 => TimeSpan.FromMinutes(15),  // 15 minutes
            _ => TimeSpan.FromMinutes(30)   // 30 minutes
        };
    }
    
    /// <summary>
    /// Increment payment attempt counter
    /// </summary>
    public static void IncrementAttempt(Order order)
    {
        order.PaymentAttempts++;
        order.LastPaymentAttempt = DateTime.UtcNow;
    }
}
