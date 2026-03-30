using elearn_server.Domain.Entities;
using elearn_server.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class QuizRepository(AppDbContext context) : IQuizRepository
{
    public Task<Quiz?> GetQuizByIdAsync(int quizId) =>
        context.Quizzes.SingleOrDefaultAsync(q => q.Id == quizId);

    public Task<Quiz?> GetQuizByIdWithQuestionsAsync(int quizId) =>
        context.Quizzes
            .Include(q => q.Questions!)
                .ThenInclude(qq => qq.Options)
            .SingleOrDefaultAsync(q => q.Id == quizId);

    public Task<QuizQuestion?> GetQuestionByIdAsync(int questionId) =>
        context.QuizQuestions.SingleOrDefaultAsync(q => q.Id == questionId);

    public Task<QuizQuestion?> GetQuestionByIdWithOptionsAsync(int questionId) =>
        context.QuizQuestions
            .Include(q => q.Options)
            .SingleOrDefaultAsync(q => q.Id == questionId);

    public Task<QuizAttempt?> GetAttemptByIdAsync(int attemptId) =>
        context.QuizAttempts.SingleOrDefaultAsync(a => a.Id == attemptId);

    public Task<QuizAttempt?> GetAttemptByIdWithDetailsAsync(int attemptId) =>
        context.QuizAttempts
            .Include(a => a.Quiz)
                .ThenInclude(q => q!.Questions!)
                    .ThenInclude(qq => qq.Options)
            .Include(a => a.Answers)
            .SingleOrDefaultAsync(a => a.Id == attemptId);

    public Task<QuizAttemptAnswer?> GetAttemptAnswerAsync(int attemptId, int questionId) =>
        context.QuizAttemptAnswers.SingleOrDefaultAsync(a => a.AttemptId == attemptId && a.QuestionId == questionId);

    public Task<List<QuizAttempt>> GetAttemptsByQuizAndUserAsync(int quizId, int userId) =>
        context.QuizAttempts
            .Where(a => a.QuizId == quizId && a.UserId == userId)
            .OrderByDescending(a => a.AttemptNumber)
            .ToListAsync();

    public Task<bool> CourseExistsAsync(int courseId) => context.Courses.AnyAsync(c => c.CourseId == courseId);
    public Task<bool> IsUserEnrolledAsync(int userId, int courseId) => context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
    public Task<bool> UserExistsAsync(int userId) => context.Users.AnyAsync(u => u.UserId == userId);

    public Task AddQuizAsync(Quiz quiz) => context.Quizzes.AddAsync(quiz).AsTask();
    public Task AddQuestionAsync(QuizQuestion question) => context.QuizQuestions.AddAsync(question).AsTask();
    public Task AddAttemptAsync(QuizAttempt attempt) => context.QuizAttempts.AddAsync(attempt).AsTask();
    public Task AddAttemptAnswerAsync(QuizAttemptAnswer answer) => context.QuizAttemptAnswers.AddAsync(answer).AsTask();
    public void RemoveQuiz(Quiz quiz) => context.Quizzes.Remove(quiz);
    public void RemoveQuestion(QuizQuestion question) => context.QuizQuestions.Remove(question);
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
