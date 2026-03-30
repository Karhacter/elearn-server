using System.Security.Claims;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api")]
[Authorize(Roles = "Student,Admin")]
public class ProgressController(IProgressService progressService) : ApiControllerBase
{
    [HttpGet("my-learning")]
    public async Task<IActionResult> GetMyLearning()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return FromResult(await progressService.GetMyLearningAsync(userId));
    }

    [HttpGet("courses/{id}/progress")]
    public async Task<IActionResult> GetCourseProgress(int id)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return FromResult(await progressService.GetCourseProgressAsync(userId, id));
    }

    [HttpPost("lessons/{id}/complete")]
    public async Task<IActionResult> CompleteLesson(int id)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return FromResult(await progressService.CompleteLessonAsync(userId, id));
    }

    [HttpPost("lessons/{id}/resume-position")]
    public async Task<IActionResult> SaveResumePosition(int id, [FromBody] LessonResumePositionRequest request)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return FromResult(await progressService.UpdateResumePositionAsync(userId, id, request));
    }

    private bool TryGetUserId(out int userId)
    {
        userId = 0;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out userId);
    }
}
