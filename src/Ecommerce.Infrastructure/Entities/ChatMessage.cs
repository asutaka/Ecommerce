using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Infrastructure.Entities;

public class ChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid SessionId { get; set; }
    public ChatSession Session { get; set; } = null!;
    
    /// <summary>
    /// Type: customer, admin, ai
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string SenderType { get; set; } = string.Empty;
    
    /// <summary>
    /// Admin ID if sender is admin, null otherwise
    /// </summary>
    public Guid? SenderId { get; set; }
    
    [MaxLength(200)]
    public string? SenderName { get; set; }
    
    [Required]
    public string Message { get; set; } = string.Empty;
    
    public bool IsRead { get; set; } = false;
    
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
