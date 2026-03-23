using elearn_server.Infrastructure.Persistence;
using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using elearn_server.Domain.Interfaces;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class CommentRepository(AppDbContext context) : ICommentRepository
{
    public Task<List<Comment>> GetAllAsync() => context.Comments.AsNoTracking().ToListAsync();
    public Task<Comment?> GetByIdAsync(int id) => context.Comments.SingleOrDefaultAsync(c => c.Id == id);
    public Task<User?> GetUserAsync(int userId) => context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
    public Task<Course?> GetCourseAsync(int courseId) => context.Courses.SingleOrDefaultAsync(c => c.CourseId == courseId);
    public Task AddAsync(Comment comment) => context.Comments.AddAsync(comment).AsTask();
    public void Remove(Comment comment) => context.Comments.Remove(comment);
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}

