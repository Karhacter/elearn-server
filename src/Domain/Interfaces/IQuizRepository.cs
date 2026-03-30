using elearn_server.Domain.Entities;

namespace elearn_server.Domain.Interfaces;

public interface IQuizRepository
{
    Task<Quiz?> GetQuizByIdAsync(int quizId);
    Task<Quiz?> GetQuizByIdWithQuestionsAsync(int quizId);
    Task<QuizQuestion?> GetQuestionByIdAsync(int questionId);
    Task<QuizQuestion?> GetQuestionByIdWithOptionsAsync(int questionId);
    Task<QuizAttempt?> GetAttemptByIdAsync(int attemptId);
    Task<QuizAttempt?> GetAttemptByIdWithDetailsAsync(int attemptId);
    Task<QuizAttemptAnswer?> GetAttemptAnswerAsync(int attemptId, int questionId);
    Task<List<QuizAttempt>> GetAttemptsByQuizAndUserAsync(int quizId, int userId);
    Task<bool> CourseExistsAsync(int courseId);
    Task<bool> IsUserEnrolledAsync(int userId, int courseId);
    Task<bool> UserExistsAsync(int userId);
    Task AddQuizAsync(Quiz quiz);
    Task AddQuestionAsync(QuizQuestion question);
    Task AddAttemptAsync(QuizAttempt attempt);
    Task AddAttemptAnswerAsync(QuizAttemptAnswer answer);
    void RemoveQuiz(Quiz quiz);
    void RemoveQuestion(QuizQuestion question);
    Task SaveChangesAsync();
}
