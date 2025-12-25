using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace Ecommerce.Web.Hubs;

public class ChatHub : Hub
{
    private readonly EcommerceDbContext _db;
    private readonly IChatService _chatService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        EcommerceDbContext db, 
        IChatService chatService,
        ILogger<ChatHub> logger)
    {
        _db = db;
        _chatService = chatService;
        _logger = logger;
    }

    /// <summary>
    /// Customer sends message
    /// </summary>
    public async Task SendMessageFromCustomer(string sessionToken, string message, string? customerName = null)
    {
        try
        {
            // Get or create session
            var session = await _chatService.GetOrCreateSessionAsync(sessionToken, customerName);

            // Save customer message
            var chatMessage = await _chatService.AddMessageAsync(
                session.Id,
                "customer",
                message,
                customerName ?? "Khách hàng"
            );

            // Check if admin is online
            var onlineAdmin = await _chatService.GetOnlineAdminAsync();

            if (onlineAdmin != null && session.AssignedAdminId == null)
            {
                // Assign session to admin
                await _chatService.AssignSessionToAdminAsync(session.Id, onlineAdmin.Id);

                // Notify admin of new message
                await Clients.User(onlineAdmin.Id.ToString())
                    .SendAsync("ReceiveNewSession", sessionToken, customerName, message);
            }
            else if (onlineAdmin != null && session.AssignedAdminId == onlineAdmin.Id)
            {
                // Send to assigned admin
                await Clients.User(onlineAdmin.Id.ToString())
                    .SendAsync("ReceiveCustomerMessage", sessionToken, message, customerName, DateTime.UtcNow);
            }
            else
            {
                // No admin online - use AI
                session.IsAiHandling = true;
                await _db.SaveChangesAsync();

                var aiResponse = await _chatService.GetAIResponseAsync(session.Id, message);

                // Save AI message
                await _chatService.AddMessageAsync(session.Id, "ai", aiResponse, "AI Assistant");

                // Send AI response to customer
                await Clients.Caller.SendAsync("ReceiveMessage", aiResponse, "AI Assistant", DateTime.UtcNow);
            }

            _logger.LogInformation("Message from customer in session {SessionToken}", sessionToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing customer message");
            await Clients.Caller.SendAsync("Error", "Có lỗi xảy ra. Vui lòng thử lại.");
        }
    }

    /// <summary>
    /// Admin sends message
    /// </summary>
    /// <summary>
    /// Admin sends message
    /// </summary>
    public async Task SendMessageFromAdmin(string sessionToken, string message)
    {
        try
        {
            // Find the specific identity that has the "Admin" role
            var adminIdentity = Context.User?.Identities
                .FirstOrDefault(i => i.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin"));

            var adminIdStr = adminIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(adminIdStr))
                 throw new Exception("Admin not authenticated (No Admin identity found)");
                 
            var adminId = Guid.Parse(adminIdStr);
            var admin = await _db.AdminUsers.FindAsync(adminId);

            if (admin == null)
            {
                await Clients.Caller.SendAsync("Error", "Admin không tồn tại trong DB");
                return;
            }

            var session = await _db.ChatSessions
                .FirstOrDefaultAsync(s => s.SessionToken == sessionToken);

            if (session == null)
            {
                await Clients.Caller.SendAsync("Error", "Session không tồn tại");
                return;
            }

            // Save admin message
            await _chatService.AddMessageAsync(
                session.Id,
                "admin",
                message,
                admin.FullName,
                adminId
            );

            // Send to customer
            await Clients.Group($"session_{sessionToken}")
                .SendAsync("ReceiveMessage", message, admin.FullName, DateTime.UtcNow);

            // Also send to admin for confirmation
            await Clients.Caller.SendAsync("MessageSent", sessionToken, message, DateTime.UtcNow);

            _logger.LogInformation("Message from admin {AdminId} in session {SessionToken}", adminId, sessionToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing admin message");
            await Clients.Caller.SendAsync("Error", "Có lỗi xảy ra khi gửi tin nhắn");
        }
    }

    /// <summary>
    /// Customer joins chat session
    /// </summary>
    public async Task JoinChat(string sessionToken)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"session_{sessionToken}");
        _logger.LogInformation("Customer joined session {SessionToken}", sessionToken);
    }

    /// <summary>
    /// Admin joins to monitor sessions
    /// </summary>
    public async Task JoinAdminHub()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "admins");
        _logger.LogInformation("Admin joined hub");
    }

    /// <summary>
    /// Admin sets online/offline status
    /// </summary>
    public async Task SetAdminOnline(bool isOnline)
    {
        try
        {
            // Find the specific identity that has the "Admin" role
            var adminIdentity = Context.User?.Identities
                .FirstOrDefault(i => i.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin"));

            var adminIdStr = adminIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(adminIdStr)) return;

            var adminId = Guid.Parse(adminIdStr);
            await _chatService.UpdateAdminOnlineStatusAsync(adminId, isOnline);

            _logger.LogInformation("Admin {AdminId} set status to {Status}", adminId, isOnline ? "online" : "offline");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting admin online status");
        }
    }

    /// <summary>
    /// Get active chat sessions (for admin)
    /// </summary>
    public async Task<List<object>> GetActiveSessions()
    {
        var sessions = await _chatService.GetActiveSessionsAsync();

        return sessions.Select(s => new
        {
            s.SessionToken,
            s.CustomerName,
            s.CustomerEmail,
            s.LastMessageAt,
            s.IsAiHandling,
            LastMessage = s.Messages.LastOrDefault()?.Message
        }).ToList<object>();
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
