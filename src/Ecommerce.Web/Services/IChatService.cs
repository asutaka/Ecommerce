using Ecommerce.Infrastructure.Entities;

namespace Ecommerce.Web.Services;

public interface IChatService
{
    Task<ChatSession> GetOrCreateSessionAsync(string sessionToken, string? customerName = null, string? customerEmail = null);
    Task<ChatMessage> AddMessageAsync(Guid sessionId, string senderType, string message, string? senderName = null, Guid? senderId = null);
    Task<string> GetAIResponseAsync(Guid sessionId, string userMessage);
    Task<AdminUser?> GetOnlineAdminAsync();
    Task UpdateAdminOnlineStatusAsync(Guid adminId, bool isOnline);
    Task<List<ChatSession>> GetActiveSessionsAsync();
    Task<List<ChatMessage>> GetSessionHistoryAsync(Guid sessionId, int take = 50);
    Task<ChatSession?> GetSessionByTokenAsync(string sessionToken);
    Task AssignSessionToAdminAsync(Guid sessionId, Guid adminId);
    Task CloseSessionAsync(Guid sessionId);
}
