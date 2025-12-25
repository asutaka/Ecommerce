using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Infrastructure.Entities;

public class ChatSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Unique token for anonymous users
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string SessionToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Link to customer if logged in
    /// </summary>
    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    
    [MaxLength(200)]
    public string? CustomerName { get; set; }
    
    [MaxLength(200)]
    public string? CustomerEmail { get; set; }
    
    /// <summary>
    /// Status: active, waiting, closed
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "active";
    
    /// <summary>
    /// Assigned admin for this chat
    /// </summary>
    public Guid? AssignedAdminId { get; set; }
    public AdminUser? AssignedAdmin { get; set; }
    
    /// <summary>
    /// True if AI is handling this chat
    /// </summary>
    public bool IsAiHandling { get; set; } = false;
    
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
    
    // Navigation
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
