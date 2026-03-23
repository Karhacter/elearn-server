using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public interface ICommentRepository
{
    Task<List<Comment>> GetAllAsync();
    Task<Comment?> GetByIdAsync(int id);
    Task<User?> GetUserAsync(int userId);
    Task<Course?> GetCourseAsync(int courseId);
    Task AddAsync(Comment comment);
    void Remove(Comment comment);
    Task SaveChangesAsync();
}

