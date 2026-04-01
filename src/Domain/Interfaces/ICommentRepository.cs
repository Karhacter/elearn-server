using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public interface ICommentRepository
{
    Task<List<Comment>> GetAllAsync();
    Task<Comment?> GetByIdAsync(int id);
    Task<Rating?> GetRatingByIdAsync(int ratingId);
    Task<Rating?> GetRatingByUserAndCourseAsync(int userId, int courseId);
    Task<List<Rating>> GetApprovedRatingsByCourseAsync(int courseId, int? star, string sortBy);
    Task<List<Rating>> GetApprovedRatingsByCourseAsync(int courseId);
    Task<Comment?> GetReviewCommentByRatingIdAsync(int ratingId);
    Task<Comment?> GetReviewCommentByUserAndCourseAsync(int userId, int courseId);
    Task<bool> IsUserEnrolledAsync(int userId, int courseId);
    Task<User?> GetUserAsync(int userId);
    Task<Course?> GetCourseAsync(int courseId);
    Task AddRatingAsync(Rating rating);
    Task AddAsync(Comment comment);
    void Remove(Comment comment);
    Task SaveChangesAsync();
}

