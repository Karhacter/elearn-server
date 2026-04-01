using System.Security.Claims;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/comments")]
public class CommentController(ICommentService commentService) : ApiControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllComments() => FromResult(await commentService.GetAllAsync());

    [HttpPost]
    [Authorize(Roles = "Student,Instructor,Admin")]
    public async Task<IActionResult> AddComment([FromBody] CommentCreateRequest request) => FromResult(await commentService.CreateAsync(request));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Student,Instructor")]
    public async Task<IActionResult> DeleteComment(int id) => FromResult(await commentService.DeleteAsync(id));

    [HttpPost("courses/{courseId}/reviews")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> CreateOrUpdateReview(int courseId, [FromBody] ReviewUpsertRequest request)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return FromResult(await commentService.CreateOrUpdateReviewAsync(userId, courseId, request));
    }

    [HttpGet("courses/{courseId}/reviews")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourseReviews(int courseId, [FromQuery] int? star, [FromQuery] string? sortBy) =>
        FromResult(await commentService.GetCourseReviewsAsync(courseId, star, sortBy));

    [HttpGet("courses/{courseId}/ratings/summary")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourseRatingSummary(int courseId) =>
        FromResult(await commentService.GetCourseRatingSummaryAsync(courseId));

    [HttpPost("reviews/{ratingId}/reply")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> ReplyToReview(int ratingId, [FromBody] ReviewReplyRequest request) =>
        FromResult(await commentService.ReplyToReviewAsync(ratingId, request));

    [HttpPatch("reviews/{ratingId}/status")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> ModerateReview(int ratingId, [FromBody] ReviewModerationRequest request) =>
        FromResult(await commentService.ModerateReviewAsync(ratingId, request));

    private bool TryGetUserId(out int userId)
    {
        userId = 0;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out userId);
    }
}
