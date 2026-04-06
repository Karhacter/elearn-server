using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories.IRepository;

public interface ICertificateRepository
{
    Task<List<Certificate>> GetAllAsync();
    Task<Certificate?> GetByIdAsync(int id);
    Task<Certificate?> GetByVerificationCodeAsync(string verificationCode);
    Task<Certificate?> GetByUserAndCourseAsync(int userId, int courseId);
    Task<User?> GetUserAsync(int userId);
    Task<Course?> GetCourseAsync(int courseId);
    Task<bool> IsUserEnrolledAsync(int userId, int courseId);
    Task<int> GetTotalLessonsByCourseAsync(int courseId);
    Task<int> GetCompletedLessonsByUserAndCourseAsync(int userId, int courseId);
    Task<int?> GetFinalQuizIdAsync(int courseId);
    Task<bool> HasPassedQuizAsync(int userId, int quizId);
    Task<bool> VerificationCodeExistsAsync(string verificationCode);
    Task AddAsync(Certificate certificate);
    Task SaveChangesAsync();
}

