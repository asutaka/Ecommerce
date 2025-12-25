namespace Ecommerce.Infrastructure.Entities;

public class AdminOnlineStatus
{
    public Guid AdminId { get; set; }
    public AdminUser Admin { get; set; } = null!;
    
    public bool IsOnline { get; set; } = false;
    
    public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
