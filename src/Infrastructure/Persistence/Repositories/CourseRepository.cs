using elearn_server.Infrastructure.Persistence;
using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using elearn_server.Domain.Interfaces;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class CourseRepository(AppDbContext context) : ICourseRepository
{
    private IQueryable<Course> BaseQuery() => context.Courses.Include(c => c.Genre).Include(c => c.Instructor);
    public Task<List<Course>> GetAllAsync() => BaseQuery().AsNoTracking().ToListAsync();
    public Task<List<Course>> GetPagedAsync(int pageNumber, int pageSize) => BaseQuery().AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    public Task<int> CountAsync() => context.Courses.CountAsync();
    public Task<Course?> GetByIdAsync(int id) => BaseQuery().SingleOrDefaultAsync(c => c.CourseId == id);
    public Task<List<Course>> GetByCategoryIdAsync(int categoryId) => BaseQuery().AsNoTracking().Where(c => c.GenreId == categoryId).ToListAsync();
    public Task<List<Course>> SearchAsync(string? keyword, int? genreId, int? instructorId)
    {
        var query = BaseQuery().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(c => (c.Title ?? string.Empty).Contains(keyword) || (c.Description ?? string.Empty).Contains(keyword));
        }
        if (genreId.HasValue) query = query.Where(c => c.GenreId == genreId.Value);
        if (instructorId.HasValue) query = query.Where(c => c.InstructorId == instructorId.Value);
        return query.ToListAsync();
    }
    public Task<Course?> GetByTitleAsync(string title) => context.Courses.SingleOrDefaultAsync(c => c.Title != null && c.Title.ToLower() == title.ToLower());
    public Task<bool> UserExistsAsync(int userId) => context.Users.AnyAsync(u => u.UserId == userId);
    public Task<bool> CategoryExistsAsync(int categoryId) => context.Categories.AnyAsync(c => c.Id == categoryId);
    public Task AddAsync(Course course) => context.Courses.AddAsync(course).AsTask();
    public void Remove(Course course) => context.Courses.Remove(course);
    public Task SaveChangesAsync() => context.SaveChangesAsync();
    public IQueryable<Course> RecommendationQuery() => BaseQuery().AsNoTracking();
}

