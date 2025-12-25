using Ecommerce.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminAuth")]
public class ChatController : Controller
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IChatService chatService, ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetActiveSessions()
    {
        try
        {
            var sessions = await _chatService.GetActiveSessionsAsync();
            
            var result = sessions.Select(s => new
            {
                s.Id,
                s.SessionToken,
                s.CustomerName,
                s.CustomerEmail,
                s.LastMessageAt,
                s.IsAiHandling,
                LastMessage = s.Messages.LastOrDefault()?.Message ?? "",
                UnreadCount = s.Messages.Count(m => !m.IsRead && m.SenderType == "customer")
            });

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active sessions");
            return StatusCode(500, new { error = "Failed to load sessions" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetSessionHistory(string sessionToken)
    {
        try
        {
            var session = await _chatService.GetSessionByTokenAsync(sessionToken);
            
            if (session == null)
                return NotFound(new { error = "Session not found" });

            var messages = session.Messages.Select(m => new
            {
                m.Id,
                m.SenderType,
                m.SenderName,
                m.Message,
                m.SentAt,
                m.IsRead
            }).OrderBy(m => m.SentAt);

            return Json(new
            {
                session = new
                {
                    session.SessionToken,
                    session.CustomerName,
                    session.CustomerEmail,
                    session.IsAiHandling
                },
                messages
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting session history for {SessionToken}", sessionToken);
            return StatusCode(500, new { error = "Failed to load session history" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CloseSession(Guid sessionId)
    {
        try
        {
            await _chatService.CloseSessionAsync(sessionId);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing session {SessionId}", sessionId);
            return StatusCode(500, new { error = "Failed to close session" });
        }
    }
}
