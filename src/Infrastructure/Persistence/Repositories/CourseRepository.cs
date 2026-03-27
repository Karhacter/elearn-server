using elearn_server.Infrastructure.Persistence;
using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using elearn_server.Domain.Interfaces;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class CourseRepository(AppDbContext context) : ICourseRepository
{
    private IQueryable<Course> BaseQuery() => context.Courses
        .Include(c => c.Genre)
        .Include(c => c.Instructor)
        .Include(c => c.LearningOutcomes)
        .Include(c => c.Requirements)
        .Include(c => c.TargetAudiences);
    private IQueryable<Course> StructureQuery() => context.Courses
        .Include(c => c.Sections)
            .ThenInclude(s => s.Lessons)
        .Include(c => c.LearningOutcomes)
        .Include(c => c.Requirements)
        .Include(c => c.TargetAudiences);
    public Task<List<Course>> GetAllAsync() => BaseQuery().AsNoTracking().ToListAsync();
    public Task<List<Course>> GetPagedAsync(int pageNumber, int pageSize) => BaseQuery().AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    public Task<int> CountAsync() => context.Courses.CountAsync();
    public Task<Course?> GetByIdAsync(int id) => BaseQuery().SingleOrDefaultAsync(c => c.CourseId == id);
    public Task<Course?> GetByIdWithStructureAsync(int id) => StructureQuery().SingleOrDefaultAsync(c => c.CourseId == id);
    public Task<Course?> GetBySlugAsync(string slug) => BaseQuery().SingleOrDefaultAsync(c => c.Slug != null && c.Slug.ToLower() == slug.ToLower());
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
    public Task AddSectionAsync(CourseSection section) => context.CourseSections.AddAsync(section).AsTask();
    public void RemoveSection(CourseSection section) => context.CourseSections.Remove(section);
    public Task AddLessonAsync(Lesson lesson) => context.Lessons.AddAsync(lesson).AsTask();
    public void RemoveLesson(Lesson lesson) => context.Lessons.Remove(lesson);
    public Task<CourseSection?> GetSectionByIdAsync(int sectionId) =>
        context.CourseSections.Include(s => s.Lessons).SingleOrDefaultAsync(s => s.SectionId == sectionId);
    public Task<Lesson?> GetLessonByIdAsync(int lessonId) =>
        context.Lessons.SingleOrDefaultAsync(l => l.LessonId == lessonId);
    public Task<List<CourseSection>> GetSectionsByCourseIdAsync(int courseId) =>
        context.CourseSections.Include(s => s.Lessons).Where(s => s.CourseId == courseId).ToListAsync();
    public Task<List<Lesson>> GetLessonsBySectionIdAsync(int sectionId) =>
        context.Lessons.Where(l => l.SectionId == sectionId).ToListAsync();
    public Task SaveChangesAsync() => context.SaveChangesAsync();
    public IQueryable<Course> RecommendationQuery() => BaseQuery().AsNoTracking();
}

