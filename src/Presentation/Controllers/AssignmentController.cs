using System.Security.Claims;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/assignments")]
[Authorize]
public class AssignmentController(IAssignmentService assignmentService) : ApiControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> CreateAssignment([FromBody] AssignmentUpsertRequest request) =>
        FromResult(await assignmentService.CreateAssignmentAsync(request));

    [HttpPut("{assignmentId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> UpdateAssignment(int assignmentId, [FromBody] AssignmentUpsertRequest request) =>
        FromResult(await assignmentService.UpdateAssignmentAsync(assignmentId, request));

    [HttpDelete("{assignmentId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> DeleteAssignment(int assignmentId) =>
        FromResult(await assignmentService.DeleteAssignmentAsync(assignmentId));

    [HttpGet("course/{courseId}")]
    [Authorize(Roles = "Admin,Instructor,Student")]
    public async Task<IActionResult> GetAssignmentsByCourse(int courseId) =>
        FromResult(await assignmentService.GetAssignmentsByCourseAsync(courseId));

    [HttpPost("{assignmentId}/submissions")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> SubmitAssignment(int assignmentId, [FromForm] AssignmentSubmissionRequest request, IFormFile? file, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return FromResult(await assignmentService.SubmitAssignmentAsync(assignmentId, userId, request, file, cancellationToken));
    }

    [HttpGet("{assignmentId}/submissions/me")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> GetMySubmission(int assignmentId)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return FromResult(await assignmentService.GetMySubmissionAsync(assignmentId, userId));
    }

    [HttpGet("{assignmentId}/submissions")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> GetSubmissions(int assignmentId) =>
        FromResult(await assignmentService.GetSubmissionsAsync(assignmentId));

    [HttpPut("submissions/{submissionId}/grade")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> GradeSubmission(int submissionId, [FromBody] AssignmentGradeRequest request)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return FromResult(await assignmentService.GradeSubmissionAsync(submissionId, userId, request));
    }

    private bool TryGetUserId(out int userId)
    {
        userId = 0;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out userId);
    }
}
