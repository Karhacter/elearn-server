using elearn_server.Domain.Entities;
using elearn_server.Domain.Enums;

using Microsoft.EntityFrameworkCore;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class CommentRepository(AppDbContext context) : ICommentRepository
{
    public Task<List<Comment>> GetAllAsync() => context.Comments.AsNoTracking().ToListAsync();

    public Task<Comment?> GetByIdAsync(int id) => context.Comments.SingleOrDefaultAsync(c => c.Id == id);

    public Task<Rating?> GetRatingByIdAsync(int ratingId) =>
        context.Ratings
            .Include(r => r.User)
            .Include(r => r.Comment)
            .SingleOrDefaultAsync(r => r.RatingId == ratingId);

    public Task<Rating?> GetRatingByUserAndCourseAsync(int userId, int courseId) =>
        context.Ratings
            .Include(r => r.User)
            .Include(r => r.Comment)
            .SingleOrDefaultAsync(r => r.UserId == userId && r.CourseId == courseId);

    public async Task<List<Rating>> GetApprovedRatingsByCourseAsync(int courseId, int? star, string sortBy)
    {
        var query = context.Ratings
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Comment)
            .Where(r => r.CourseId == courseId && r.Status == ReviewStatus.Approved);

        if (star.HasValue)
        {
            query = query.Where(r => r.Score == star.Value);
        }

        return sortBy.Equals("highest", StringComparison.OrdinalIgnoreCase)
            ? await query.OrderByDescending(r => r.Score).ThenByDescending(r => r.CreatedAt).ToListAsync()
            : await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
    }

    public Task<List<Rating>> GetApprovedRatingsByCourseAsync(int courseId) =>
        context.Ratings
            .AsNoTracking()
            .Where(r => r.CourseId == courseId && r.Status == ReviewStatus.Approved)
            .ToListAsync();

    public Task<Comment?> GetReviewCommentByRatingIdAsync(int ratingId) =>
        context.Comments.SingleOrDefaultAsync(c => c.RatingId == ratingId);

    public Task<Comment?> GetReviewCommentByUserAndCourseAsync(int userId, int courseId) =>
        context.Comments
            .OrderByDescending(c => c.CommentDate)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId);

    public Task<bool> IsUserEnrolledAsync(int userId, int courseId) =>
        context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId && e.IsActive);

    public Task<User?> GetUserAsync(int userId) => context.Users.SingleOrDefaultAsync(u => u.UserId == userId);

    public Task<Course?> GetCourseAsync(int courseId) => context.Courses.SingleOrDefaultAsync(c => c.CourseId == courseId);

    public Task AddRatingAsync(Rating rating) => context.Ratings.AddAsync(rating).AsTask();

    public Task AddAsync(Comment comment) => context.Comments.AddAsync(comment).AsTask();

    public void Remove(Comment comment) => context.Comments.Remove(comment);

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
