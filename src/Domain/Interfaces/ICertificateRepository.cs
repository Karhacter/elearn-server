using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public interface ICertificateRepository
{
    Task<List<Certificate>> GetAllAsync();
    Task<User?> GetUserAsync(int userId);
    Task<Course?> GetCourseAsync(int courseId);
    Task AddAsync(Certificate certificate);
    Task SaveChangesAsync();
}

