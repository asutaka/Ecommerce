using Ecommerce.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BannerAnalyticsController(IBannerAnalyticsService analyticsService) : ControllerBase
{
    private readonly IBannerAnalyticsService _analyticsService = analyticsService;

    /// <summary>
    /// Track a banner view (impression)
    /// </summary>
    [HttpPost("track-view")]
    public async Task<IActionResult> TrackView([FromBody] TrackRequest request)
    {
        try
        {
            if (request.BannerId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Invalid banner ID" });
            }

            await _analyticsService.TrackViewAsync(request.BannerId);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            // Log error but don't expose internal details
            return StatusCode(500, new { success = false, message = "Failed to track view" });
        }
    }

    /// <summary>
    /// Track a banner click
    /// </summary>
    [HttpPost("track-click")]
    public async Task<IActionResult> TrackClick([FromBody] TrackRequest request)
    {
        try
        {
            if (request.BannerId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Invalid banner ID" });
            }

            await _analyticsService.TrackClickAsync(request.BannerId);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            // Log error but don't expose internal details
            return StatusCode(500, new { success = false, message = "Failed to track click" });
        }
    }
}

/// <summary>
/// Request model for tracking
/// </summary>
public class TrackRequest
{
    public Guid BannerId { get; set; }
}
