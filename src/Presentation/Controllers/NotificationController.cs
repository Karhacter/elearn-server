using System.Security.Claims;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/notifications")]
[Authorize]
public class NotificationController(INotificationService notificationService) : ApiControllerBase
{
    // ─── GET api/notifications ──────────────────────────────────────────────────
    /// <summary>Returns all notifications for the currently authenticated user.</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        return FromResult(await notificationService.GetByUserAsync(userId));
    }

    // ─── GET api/notifications/summary ──────────────────────────────────────────
    /// <summary>Returns total and unread notification counts (for badge display).</summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        return FromResult(await notificationService.GetSummaryAsync(userId));
    }

    // ─── POST api/notifications ──────────────────────────────────────────────────
    /// <summary>Creates a notification for a target user. Admin-only.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateNotificationRequest request) =>
        FromResult(await notificationService.CreateAsync(request));

    // ─── PATCH api/notifications/{id}/read ──────────────────────────────────────
    /// <summary>Marks a specific notification as read.</summary>
    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        return FromResult(await notificationService.MarkAsReadAsync(id, userId));
    }

    // ─── PATCH api/notifications/read-all ───────────────────────────────────────
    /// <summary>Marks all unread notifications of the current user as read.</summary>
    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        return FromResult(await notificationService.MarkAllAsReadAsync(userId));
    }

    // ─── DELETE api/notifications/{id} ──────────────────────────────────────────
    /// <summary>Soft-deletes a notification belonging to the current user.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        return FromResult(await notificationService.DeleteAsync(id, userId));
    }

    // ─── Helpers ────────────────────────────────────────────────────────────────
    private bool TryGetUserId(out int userId)
    {
        userId = 0;
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out userId);
    }
}
