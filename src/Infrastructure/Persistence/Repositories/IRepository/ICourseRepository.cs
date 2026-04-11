using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories.IRepository;

public interface ICourseRepository
{
    Task<List<Course>> GetAllAsync();
    Task<List<Course>> GetPagedAsync(int pageNumber, int pageSize);
    Task<List<Course>> GetDeletedAsync(int pageNumber, int pageSize);
    Task<int> CountAsync();
    Task<Course?> GetByIdAsync(int id);
    Task<Course?> GetByIdIncludingDeletedAsync(int id);
    Task<Course?> GetByIdWithStructureAsync(int id);
    Task<Course?> GetBySlugAsync(string slug);
    Task<List<Course>> GetByCategoryIdAsync(int categoryId);
    Task<List<Course>> SearchAsync(string? keyword, int? genreId, int? instructorId);
    Task<Course?> GetByTitleAsync(string title);
    Task<bool> UserExistsAsync(int userId);
    Task<bool> CategoryExistsAsync(int categoryId);
    Task AddAsync(Course course);
    void Remove(Course course);

    // For Sections | Lessons
    Task AddSectionAsync(CourseSection section);
    void RemoveSection(CourseSection section);
    Task AddLessonAsync(Lesson lesson);
    void RemoveLesson(Lesson lesson);
    Task<CourseSection?> GetSectionByIdAsync(int sectionId);

    //
    Task<List<Lesson>> GetLessonsBySectionIdIncludingDeletedAsync(int sectionId);

    Task<CourseSection?> GetSectionByIdIncludingDeletedAsync(int sectionId);
    Task<Lesson?> GetLessonByIdAsync(int lessonId);
    Task<Lesson?> GetLessonByIdIncludingDeletedAsync(int lessonId);
    Task<List<CourseSection>> GetSectionsByCourseIdAsync(int courseId);
    Task<List<CourseSection>> GetSectionsByCourseIdIncludingDeletedAsync(int courseId);
    Task<List<CourseSection>> GetDeletedSectionsByCourseIdAsync(int courseId);

    Task<List<Lesson>> GetLessonsPagedBySectionIdAsync(int sectionId, int page, int pageSize);
    Task<int> CountLessonsBySectionIdAsync(int sectionId);
    Task<List<Lesson>> GetLessonsBySectionIdAsync(int sectionId);
    Task SaveChangesAsync();
    IQueryable<Course> RecommendationQuery();
}
