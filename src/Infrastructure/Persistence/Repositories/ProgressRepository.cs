using elearn_server.Domain.Entities;
using elearn_server.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class ProgressRepository(AppDbContext context) : IProgressRepository
{
    public Task<Lesson?> GetLessonByIdAsync(int lessonId) =>
        context.Lessons
            .Include(l => l.CourseSection)
            .SingleOrDefaultAsync(l => l.LessonId == lessonId);

    public Task<Course?> GetCourseWithStructureAsync(int courseId) =>
        context.Courses
            .Include(c => c.Sections)
                .ThenInclude(s => s.Lessons)
            .SingleOrDefaultAsync(c => c.CourseId == courseId);

    public Task<List<Enrollment>> GetEnrollmentsByUserIdAsync(int userId) =>
        context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.UserId == userId)
            .ToListAsync();

    public Task<bool> IsUserEnrolledAsync(int userId, int courseId) =>
        context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);

    public Task<CourseProgress?> GetCourseProgressAsync(int userId, int courseId) =>
        context.CourseProgresses
            .Include(cp => cp.Course)
            .SingleOrDefaultAsync(cp => cp.UserId == userId && cp.CourseId == courseId);

    public Task<LessonProgress?> GetLessonProgressAsync(int userId, int lessonId) =>
        context.LessonProgresses
            .SingleOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == lessonId);

    public Task<LessonCompletion?> GetLessonCompletionAsync(int userId, int lessonId) =>
        context.LessonCompletions
            .SingleOrDefaultAsync(lc => lc.UserId == userId && lc.LessonId == lessonId);

    public Task<List<LessonCompletion>> GetLessonCompletionsForCourseAsync(int userId, int courseId) =>
        context.LessonCompletions
            .Where(lc => lc.UserId == userId && lc.CourseId == courseId)
            .ToListAsync();

    public Task<List<LessonProgress>> GetLessonProgressesForCourseAsync(int userId, int courseId) =>
        context.LessonProgresses
            .Where(lp => lp.UserId == userId && lp.CourseId == courseId)
            .ToListAsync();

    public Task<int> CountLessonsForCourseAsync(int courseId) =>
        context.Lessons.CountAsync(l => l.CourseSection != null && l.CourseSection.CourseId == courseId);

    public Task AddCourseProgressAsync(CourseProgress progress) => context.CourseProgresses.AddAsync(progress).AsTask();
    public Task AddLessonProgressAsync(LessonProgress progress) => context.LessonProgresses.AddAsync(progress).AsTask();
    public Task AddLessonCompletionAsync(LessonCompletion completion) => context.LessonCompletions.AddAsync(completion).AsTask();
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
