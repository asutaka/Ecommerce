using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Infrastructure.Entities;

public class ChatSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public bool EnableAI { get; set; } = true;
    
    [MaxLength(50)]
    public string AiProvider { get; set; } = "groq"; // groq, gemini, openai
    
    [MaxLength(100)]
    public string AiModel { get; set; } = "llama3-8b-8192";
    
    public string? SystemPrompt { get; set; }
    
    public bool AutoReplyWhenOffline { get; set; } = true;
    
    public string OfflineMessage { get; set; } = "Xin chào! Admin hiện không online. Tôi là AI assistant, tôi có thể giúp gì cho bạn?";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
