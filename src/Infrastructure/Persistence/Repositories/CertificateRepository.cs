using elearn_server.Domain.Entities;
using elearn_server.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class CertificateRepository(AppDbContext context) : ICertificateRepository
{
    public Task<List<Certificate>> GetAllAsync() => context.Certificates.AsNoTracking().ToListAsync();

    public Task<Certificate?> GetByIdAsync(int id) =>
        context.Certificates.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);

    public Task<Certificate?> GetByVerificationCodeAsync(string verificationCode) =>
        context.Certificates.AsNoTracking().SingleOrDefaultAsync(c => c.VerificationCode == verificationCode);

    public Task<Certificate?> GetByUserAndCourseAsync(int userId, int courseId) =>
        context.Certificates.AsNoTracking().SingleOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId);

    public Task<User?> GetUserAsync(int userId) => context.Users.SingleOrDefaultAsync(u => u.UserId == userId);

    public Task<Course?> GetCourseAsync(int courseId) => context.Courses.SingleOrDefaultAsync(c => c.CourseId == courseId);

    public Task<bool> IsUserEnrolledAsync(int userId, int courseId) =>
        context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId && e.IsActive);

    public Task<int> GetTotalLessonsByCourseAsync(int courseId) =>
        context.Lessons.CountAsync(l => l.CourseId == courseId);

    public Task<int> GetCompletedLessonsByUserAndCourseAsync(int userId, int courseId) =>
        context.LessonCompletions
            .Where(lc => lc.UserId == userId && lc.CourseId == courseId)
            .Select(lc => lc.LessonId)
            .Distinct()
            .CountAsync();

    public Task<int?> GetFinalQuizIdAsync(int courseId) =>
        context.Quizzes
            .Where(q => q.CourseId == courseId)
            .OrderByDescending(q => q.Id)
            .Select(q => (int?)q.Id)
            .FirstOrDefaultAsync();

    public Task<bool> HasPassedQuizAsync(int userId, int quizId) =>
        context.QuizAttempts.AnyAsync(a =>
            a.UserId == userId &&
            a.QuizId == quizId &&
            a.IsPassed &&
            (a.Status == QuizAttemptStatus.Submitted || a.Status == QuizAttemptStatus.TimedOut));

    public Task<bool> VerificationCodeExistsAsync(string verificationCode) =>
        context.Certificates.AnyAsync(c => c.VerificationCode == verificationCode);

    public Task AddAsync(Certificate certificate) => context.Certificates.AddAsync(certificate).AsTask();

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
