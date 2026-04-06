using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface IQuizService
{
    Task<ServiceResult<QuizResponse>> CreateQuizAsync(QuizUpsertRequest request);
    Task<ServiceResult<QuizResponse>> UpdateQuizAsync(int quizId, QuizUpsertRequest request);
    Task<ServiceResult<object>> DeleteQuizAsync(int quizId);
    Task<ServiceResult<QuizResponse>> CreateQuestionAsync(int quizId, QuizQuestionUpsertRequest request);
    Task<ServiceResult<QuizResponse>> UpdateQuestionAsync(int quizId, int questionId, QuizQuestionUpsertRequest request);
    Task<ServiceResult<object>> DeleteQuestionAsync(int quizId, int questionId);
    Task<ServiceResult<QuizAttemptStartResponse>> StartAttemptAsync(int quizId, int userId);
    Task<ServiceResult<QuizAttemptAnswerResponse>> SaveAnswerAsync(int attemptId, int userId, QuizAttemptAnswerRequest request);
    Task<ServiceResult<QuizAttemptResultResponse>> SubmitAttemptAsync(int attemptId, int userId);
    Task<ServiceResult<IReadOnlyCollection<QuizAttemptResultResponse>>> GetMyAttemptHistoryAsync(int quizId, int userId);
    Task<ServiceResult<QuizResultSummaryResponse>> GetMyResultSummaryAsync(int quizId, int userId);
}