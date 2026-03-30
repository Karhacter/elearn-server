using System.Security.Claims;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/quizzes")]
[Authorize]
public class QuizController(IQuizService quizService) : ApiControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> CreateQuiz([FromBody] QuizUpsertRequest request) =>
        FromResult(await quizService.CreateQuizAsync(request));

    [HttpPut("{quizId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> UpdateQuiz(int quizId, [FromBody] QuizUpsertRequest request) =>
        FromResult(await quizService.UpdateQuizAsync(quizId, request));

    [HttpDelete("{quizId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> DeleteQuiz(int quizId) =>
        FromResult(await quizService.DeleteQuizAsync(quizId));

    [HttpPost("{quizId}/questions")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> CreateQuestion(int quizId, [FromBody] QuizQuestionUpsertRequest request) =>
        FromResult(await quizService.CreateQuestionAsync(quizId, request));

    [HttpPut("{quizId}/questions/{questionId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> UpdateQuestion(int quizId, int questionId, [FromBody] QuizQuestionUpsertRequest request) =>
        FromResult(await quizService.UpdateQuestionAsync(quizId, questionId, request));

    [HttpDelete("{quizId}/questions/{questionId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> DeleteQuestion(int quizId, int questionId) =>
        FromResult(await quizService.DeleteQuestionAsync(quizId, questionId));

    [HttpPost("{quizId}/attempts/start")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> StartAttempt(int quizId)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return FromResult(await quizService.StartAttemptAsync(quizId, userId));
    }

    [HttpPost("attempts/{attemptId}/answers")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> SaveAnswer(int attemptId, [FromBody] QuizAttemptAnswerRequest request)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return FromResult(await quizService.SaveAnswerAsync(attemptId, userId, request));
    }

    [HttpPost("attempts/{attemptId}/submit")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> SubmitAttempt(int attemptId)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return FromResult(await quizService.SubmitAttemptAsync(attemptId, userId));
    }

    [HttpGet("{quizId}/attempts/me")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> GetMyAttemptHistory(int quizId)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return FromResult(await quizService.GetMyAttemptHistoryAsync(quizId, userId));
    }

    [HttpGet("{quizId}/result/me")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> GetMyResultSummary(int quizId)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return FromResult(await quizService.GetMyResultSummaryAsync(quizId, userId));
    }

    private bool TryGetUserId(out int userId)
    {
        userId = 0;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out userId);
    }
}
