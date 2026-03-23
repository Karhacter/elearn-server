using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public interface IEnrollmentRepository
{
    Task<List<Enrollment>> GetAllAsync();
    Task<Enrollment?> GetByIdAsync(int id);
    Task<Enrollment?> GetByUserAndCourseAsync(int userId, int courseId);
    Task<User?> GetUserAsync(int userId);
    Task<Course?> GetCourseAsync(int courseId);
    Task AddAsync(Enrollment enrollment);
    void Remove(Enrollment enrollment);
    Task SaveChangesAsync();
}

