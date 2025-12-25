using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Services.AI;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Services;

public class ChatService : IChatService
{
    private readonly EcommerceDbContext _db;
    private readonly IAIProvider? _aiProvider;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        EcommerceDbContext db, 
        ILogger<ChatService> logger,
        IAIProvider? aiProvider = null)
    {
        _db = db;
        _logger = logger;
        _aiProvider = aiProvider;
    }

    public async Task<ChatSession> GetOrCreateSessionAsync(string sessionToken, string? customerName = null, string? customerEmail = null)
    {
        var session = await _db.ChatSessions
            .FirstOrDefaultAsync(s => s.SessionToken == sessionToken);

        if (session == null)
        {
            session = new ChatSession
            {
                SessionToken = sessionToken,
                CustomerName = customerName ?? "Khách hàng",
                CustomerEmail = customerEmail,
                Status = "active",
                StartedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow
            };

            _db.ChatSessions.Add(session);
            await _db.SaveChangesAsync();
            
            _logger.LogInformation("Created new chat session: {SessionToken}", sessionToken);
        }
        else
        {
            // Update last message time
            session.LastMessageAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(customerName))
                session.CustomerName = customerName;
            if (!string.IsNullOrEmpty(customerEmail))
                session.CustomerEmail = customerEmail;
                
            await _db.SaveChangesAsync();
        }

        return session;
    }

    public async Task<ChatMessage> AddMessageAsync(
        Guid sessionId, 
        string senderType, 
        string message, 
        string? senderName = null, 
        Guid? senderId = null)
    {
        var chatMessage = new ChatMessage
        {
            SessionId = sessionId,
            SenderType = senderType,
            SenderId = senderId,
            SenderName = senderName,
            Message = message,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };

        _db.ChatMessages.Add(chatMessage);

        // Update session's last message time
        var session = await _db.ChatSessions.FindAsync(sessionId);
        if (session != null)
        {
            session.LastMessageAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Message added to session {SessionId} by {SenderType}", sessionId, senderType);

        return chatMessage;
    }

    public async Task<string> GetAIResponseAsync(Guid sessionId, string userMessage)
    {
        if (_aiProvider == null)
        {
            return "Xin lỗi, hệ thống AI hiện không khả dụng. Vui lòng thử lại sau.";
        }

        try
        {
            // Get conversation history for context
            var history = await GetSessionHistoryAsync(sessionId, 10);
            
            // Call AI provider
            var response = await _aiProvider.GenerateResponseAsync(userMessage, history);
            
            _logger.LogInformation("AI response generated for session {SessionId}", sessionId);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating AI response for session {SessionId}", sessionId);
            return "Xin lỗi, đã có lỗi xảy ra. Vui lòng thử lại sau hoặc đợi admin hỗ trợ.";
        }
    }

    public async Task<AdminUser?> GetOnlineAdminAsync()
    {
        var onlineStatus = await _db.AdminOnlineStatuses
            .Where(s => s.IsOnline)
            .OrderBy(s => s.LastSeenAt) // Round-robin: get admin who was online longest ago
            .FirstOrDefaultAsync();

        if (onlineStatus == null)
            return null;

        return await _db.AdminUsers.FindAsync(onlineStatus.AdminId);
    }

    public async Task UpdateAdminOnlineStatusAsync(Guid adminId, bool isOnline)
    {
        var status = await _db.AdminOnlineStatuses.FindAsync(adminId);

        if (status == null)
        {
            status = new AdminOnlineStatus
            {
                AdminId = adminId,
                IsOnline = isOnline,
                LastSeenAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.AdminOnlineStatuses.Add(status);
        }
        else
        {
            status.IsOnline = isOnline;
            status.UpdatedAt = DateTime.UtcNow;
            if (isOnline)
                status.LastSeenAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Admin {AdminId} status updated to {Status}", adminId, isOnline ? "online" : "offline");
    }

    public async Task<List<ChatSession>> GetActiveSessionsAsync()
    {
        return await _db.ChatSessions
            .Where(s => s.Status == "active")
            .OrderByDescending(s => s.LastMessageAt)
            .Include(s => s.Messages.OrderByDescending(m => m.SentAt).Take(1)) // Last message
            .ToListAsync();
    }

    public async Task<List<ChatMessage>> GetSessionHistoryAsync(Guid sessionId, int take = 50)
    {
        return await _db.ChatMessages
            .Where(m => m.SessionId == sessionId)
            .OrderByDescending(m => m.SentAt)
            .Take(take)
            .OrderBy(m => m.SentAt) // Reverse to chronological order
            .ToListAsync();
    }

    public async Task<ChatSession?> GetSessionByTokenAsync(string sessionToken)
    {
        return await _db.ChatSessions
            .Include(s => s.Messages.OrderBy(m => m.SentAt))
            .FirstOrDefaultAsync(s => s.SessionToken == sessionToken);
    }

    public async Task AssignSessionToAdminAsync(Guid sessionId, Guid adminId)
    {
        var session = await _db.ChatSessions.FindAsync(sessionId);
        if (session != null)
        {
            session.AssignedAdminId = adminId;
            session.IsAiHandling = false;
            await _db.SaveChangesAsync();
            
            _logger.LogInformation("Session {SessionId} assigned to admin {AdminId}", sessionId, adminId);
        }
    }

    public async Task CloseSessionAsync(Guid sessionId)
    {
        var session = await _db.ChatSessions.FindAsync(sessionId);
        if (session != null)
        {
            session.Status = "closed";
            session.EndedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            
            _logger.LogInformation("Session {SessionId} closed", sessionId);
        }
    }
}
