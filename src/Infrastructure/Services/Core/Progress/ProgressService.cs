using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Domain.Interfaces;

namespace elearn_server.Infrastructure.Services.Core.Progress;

public class ProgressService(IProgressRepository repository) : IProgressService
{
    public async Task<ServiceResult<IReadOnlyCollection<MyLearningItemResponse>>> GetMyLearningAsync(int userId)
    {
        var enrollments = await repository.GetEnrollmentsByUserIdAsync(userId);
        if (enrollments.Count == 0)
        {
            return ServiceResult<IReadOnlyCollection<MyLearningItemResponse>>.Fail(StatusCodes.Status404NotFound, "Không có khóa học nào trong danh sách học tập.");
        }

        var result = new List<MyLearningItemResponse>();
        foreach (var enrollment in enrollments)
        {
            if (enrollment.Course is null)
            {
                continue;
            }

            var progress = await BuildCourseProgressAsync(userId, enrollment.Course.CourseId);
            result.Add(new MyLearningItemResponse
            {
                CourseId = enrollment.Course.CourseId,
                CourseTitle = enrollment.Course.Title,
                Thumbnail = enrollment.Course.Thumbnail,
                ProgressPercent = progress.ProgressPercent,
                LastViewedLessonId = progress.LastViewedLessonId,
                LastViewedAt = progress.LastViewedAt
            });
        }

        return ServiceResult<IReadOnlyCollection<MyLearningItemResponse>>.Ok(result);
    }

    public async Task<ServiceResult<CourseProgressResponse>> GetCourseProgressAsync(int userId, int courseId)
    {
        if (!await repository.IsUserEnrolledAsync(userId, courseId))
        {
            return ServiceResult<CourseProgressResponse>.Fail(StatusCodes.Status403Forbidden, "Bạn chưa đăng ký khóa học này.");
        }

        var course = await repository.GetCourseWithStructureAsync(courseId);
        if (course is null)
        {
            return ServiceResult<CourseProgressResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        var completions = await repository.GetLessonCompletionsForCourseAsync(userId, courseId);
        var progresses = await repository.GetLessonProgressesForCourseAsync(userId, courseId);
        var totalLessons = await repository.CountLessonsForCourseAsync(courseId);
        var completedLessons = completions.Count;
        var progressPercent = totalLessons == 0 ? 0 : Math.Round(completedLessons * 100d / totalLessons, 2);

        var lastViewed = progresses
            .Where(p => p.LastViewedAt.HasValue)
            .OrderByDescending(p => p.LastViewedAt)
            .FirstOrDefault();

        await UpsertCourseProgressAsync(userId, courseId, completedLessons, totalLessons, progressPercent, lastViewed);

        var orderedLessons = GetOrderedLessons(course);
        var completionSet = completions.Select(c => c.LessonId).ToHashSet();
        var progressMap = progresses.ToDictionary(p => p.LessonId);

        var lessonResponses = orderedLessons.Select(lesson =>
        {
            progressMap.TryGetValue(lesson.LessonId, out var lessonProgress);
            return new LessonProgressResponse
            {
                LessonId = lesson.LessonId,
                Title = lesson.Title ?? string.Empty,
                SectionId = lesson.SectionId,
                Order = lesson.Order,
                IsCompleted = completionSet.Contains(lesson.LessonId),
                WatchPositionSeconds = lessonProgress?.WatchPositionSeconds ?? 0,
                LastViewedAt = lessonProgress?.LastViewedAt
            };
        }).ToList();

        return ServiceResult<CourseProgressResponse>.Ok(new CourseProgressResponse
        {
            CourseId = course.CourseId,
            CourseTitle = course.Title,
            Thumbnail = course.Thumbnail,
            ProgressPercent = progressPercent,
            CompletedLessons = completedLessons,
            TotalLessons = totalLessons,
            LastViewedLessonId = lastViewed?.LessonId,
            LastViewedAt = lastViewed?.LastViewedAt,
            Lessons = lessonResponses
        });
    }

    public async Task<ServiceResult<LessonProgressResponse>> CompleteLessonAsync(int userId, int lessonId)
    {
        var lesson = await repository.GetLessonByIdAsync(lessonId);
        if (lesson is null)
        {
            return ServiceResult<LessonProgressResponse>.Fail(StatusCodes.Status404NotFound, "Lesson not found.");
        }

        if (!await repository.IsUserEnrolledAsync(userId, lesson.CourseId))
        {
            return ServiceResult<LessonProgressResponse>.Fail(StatusCodes.Status403Forbidden, "Bạn chưa đăng ký khóa học này.");
        }

        var course = await repository.GetCourseWithStructureAsync(lesson.CourseId);
        if (course is null)
        {
            return ServiceResult<LessonProgressResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        if (course.IsSequential)
        {
            var orderedLessons = GetOrderedLessons(course);
            var index = orderedLessons.FindIndex(l => l.LessonId == lessonId);
            if (index > 0)
            {
                var completedIds = (await repository.GetLessonCompletionsForCourseAsync(userId, course.CourseId))
                    .Select(c => c.LessonId)
                    .ToHashSet();
                var requiredLessons = orderedLessons.Take(index).Select(l => l.LessonId);
                if (requiredLessons.Any(id => !completedIds.Contains(id)))
                {
                    return ServiceResult<LessonProgressResponse>.Fail(StatusCodes.Status400BadRequest, "Bạn cần hoàn thành các bài học trước đó.");
                }
            }
        }

        var completion = await repository.GetLessonCompletionAsync(userId, lessonId);
        if (completion is null)
        {
            completion = new LessonCompletion
            {
                UserId = userId,
                CourseId = lesson.CourseId,
                LessonId = lessonId,
                CompletedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await repository.AddLessonCompletionAsync(completion);
        }
        else
        {
            completion.CompletedAt = DateTime.UtcNow;
            completion.UpdatedAt = DateTime.UtcNow;
        }

        var progress = await repository.GetLessonProgressAsync(userId, lessonId);
        if (progress is null)
        {
            progress = new LessonProgress
            {
                UserId = userId,
                CourseId = lesson.CourseId,
                LessonId = lessonId,
                WatchPositionSeconds = 0,
                LastViewedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await repository.AddLessonProgressAsync(progress);
        }
        else
        {
            progress.LastViewedAt = DateTime.UtcNow;
            progress.UpdatedAt = DateTime.UtcNow;
        }

        var courseProgress = await BuildCourseProgressAsync(userId, lesson.CourseId, lessonId, progress.LastViewedAt);

        return ServiceResult<LessonProgressResponse>.Ok(new LessonProgressResponse
        {
            LessonId = lesson.LessonId,
            Title = lesson.Title ?? string.Empty,
            SectionId = lesson.SectionId,
            Order = lesson.Order,
            IsCompleted = true,
            WatchPositionSeconds = progress.WatchPositionSeconds,
            LastViewedAt = progress.LastViewedAt
        }, "Lesson completed.");
    }

    public async Task<ServiceResult<LessonProgressResponse>> UpdateResumePositionAsync(int userId, int lessonId, LessonResumePositionRequest request)
    {
        var lesson = await repository.GetLessonByIdAsync(lessonId);
        if (lesson is null)
        {
            return ServiceResult<LessonProgressResponse>.Fail(StatusCodes.Status404NotFound, "Lesson not found.");
        }

        if (!await repository.IsUserEnrolledAsync(userId, lesson.CourseId))
        {
            return ServiceResult<LessonProgressResponse>.Fail(StatusCodes.Status403Forbidden, "Bạn chưa đăng ký khóa học này.");
        }

        var progress = await repository.GetLessonProgressAsync(userId, lessonId);
        if (progress is null)
        {
            progress = new LessonProgress
            {
                UserId = userId,
                CourseId = lesson.CourseId,
                LessonId = lessonId,
                WatchPositionSeconds = request.WatchPositionSeconds,
                LastViewedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await repository.AddLessonProgressAsync(progress);
        }
        else
        {
            progress.WatchPositionSeconds = request.WatchPositionSeconds;
            progress.LastViewedAt = DateTime.UtcNow;
            progress.UpdatedAt = DateTime.UtcNow;
        }

        var completion = await repository.GetLessonCompletionAsync(userId, lessonId);
        await BuildCourseProgressAsync(userId, lesson.CourseId, lessonId, progress.LastViewedAt);

        return ServiceResult<LessonProgressResponse>.Ok(new LessonProgressResponse
        {
            LessonId = lesson.LessonId,
            Title = lesson.Title ?? string.Empty,
            SectionId = lesson.SectionId,
            Order = lesson.Order,
            IsCompleted = completion is not null,
            WatchPositionSeconds = progress.WatchPositionSeconds,
            LastViewedAt = progress.LastViewedAt
        }, "Resume position saved.");
    }

    private async Task<CourseProgress> BuildCourseProgressAsync(int userId, int courseId, int? lastViewedLessonId = null, DateTime? lastViewedAt = null)
    {
        var existing = await repository.GetCourseProgressAsync(userId, courseId);
        var completions = await repository.GetLessonCompletionsForCourseAsync(userId, courseId);
        var totalLessons = await repository.CountLessonsForCourseAsync(courseId);
        var completedLessons = completions.Count;
        var progressPercent = totalLessons == 0 ? 0 : Math.Round(completedLessons * 100d / totalLessons, 2);

        var resolvedLastViewedLessonId = lastViewedLessonId ?? existing?.LastViewedLessonId;
        var resolvedLastViewedAt = lastViewedAt ?? existing?.LastViewedAt;

        var latestProgress = resolvedLastViewedLessonId.HasValue
            ? new LessonProgress { LessonId = resolvedLastViewedLessonId.Value, LastViewedAt = resolvedLastViewedAt }
            : null;

        await UpsertCourseProgressAsync(userId, courseId, completedLessons, totalLessons, progressPercent, latestProgress);

        return new CourseProgress
        {
            UserId = userId,
            CourseId = courseId,
            CompletedLessons = completedLessons,
            TotalLessons = totalLessons,
            ProgressPercent = progressPercent,
            LastViewedLessonId = resolvedLastViewedLessonId,
            LastViewedAt = resolvedLastViewedAt
        };
    }

    private async Task UpsertCourseProgressAsync(int userId, int courseId, int completedLessons, int totalLessons, double progressPercent, LessonProgress? lastViewed)
    {
        var courseProgress = await repository.GetCourseProgressAsync(userId, courseId);
        if (courseProgress is null)
        {
            courseProgress = new CourseProgress
            {
                UserId = userId,
                CourseId = courseId,
                CompletedLessons = completedLessons,
                TotalLessons = totalLessons,
                ProgressPercent = progressPercent,
                LastViewedLessonId = lastViewed?.LessonId,
                LastViewedAt = lastViewed?.LastViewedAt,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await repository.AddCourseProgressAsync(courseProgress);
        }
        else
        {
            courseProgress.CompletedLessons = completedLessons;
            courseProgress.TotalLessons = totalLessons;
            courseProgress.ProgressPercent = progressPercent;
            if (lastViewed?.LessonId is not null)
            {
                courseProgress.LastViewedLessonId = lastViewed.LessonId;
                courseProgress.LastViewedAt = lastViewed.LastViewedAt;
            }
            courseProgress.UpdatedAt = DateTime.UtcNow;
        }

        await repository.SaveChangesAsync();
    }

    private static List<Lesson> GetOrderedLessons(Course course)
    {
        if (course.Sections is { Count: > 0 })
        {
            return course.Sections
                .OrderBy(s => s.Order)
                .SelectMany(s => (s.Lessons ?? new List<Lesson>()).OrderBy(l => l.Order))
                .ToList();
        }

        return course.Lessons?.OrderBy(l => l.Order).ToList() ?? new List<Lesson>();
    }
}
