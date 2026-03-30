using elearn_server.Domain.Entities;

namespace elearn_server.Domain.Interfaces;

public interface IProgressRepository
{
    Task<Lesson?> GetLessonByIdAsync(int lessonId);
    Task<Course?> GetCourseWithStructureAsync(int courseId);
    Task<List<Enrollment>> GetEnrollmentsByUserIdAsync(int userId);
    Task<bool> IsUserEnrolledAsync(int userId, int courseId);
    Task<CourseProgress?> GetCourseProgressAsync(int userId, int courseId);
    Task<LessonProgress?> GetLessonProgressAsync(int userId, int lessonId);
    Task<LessonCompletion?> GetLessonCompletionAsync(int userId, int lessonId);
    Task<List<LessonCompletion>> GetLessonCompletionsForCourseAsync(int userId, int courseId);
    Task<List<LessonProgress>> GetLessonProgressesForCourseAsync(int userId, int courseId);
    Task<int> CountLessonsForCourseAsync(int courseId);
    Task AddCourseProgressAsync(CourseProgress progress);
    Task AddLessonProgressAsync(LessonProgress progress);
    Task AddLessonCompletionAsync(LessonCompletion completion);
    Task SaveChangesAsync();
}
