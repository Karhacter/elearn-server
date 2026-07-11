using System.Text.RegularExpressions;
using elearn_server.Application.Common;
using elearn_server.Application.Mappings;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Domain.Enums;
using elearn_server.Application.Interfaces;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;
using Microsoft.AspNetCore.Http;
namespace elearn_server.Infrastructure.Services.Core.Courses;

public partial class CourseService
{
    // For Lessons
    public async Task<ServiceResult<PagedResult<LessonResponse>>> GetLessonsAsync(int sectionId, int page, int pageSize)
    {
        var section = await repository.GetSectionByIdAsync(sectionId);
        if (section is null)
        {
            return ServiceResult<PagedResult<LessonResponse>>.Fail(StatusCodes.Status404NotFound, "Section not found.");
        }

        page = Math.Max(1, page);
        pageSize = Math.Max(1, pageSize);

        var lessons = await repository.GetLessonsPagedBySectionIdAsync(sectionId, page, pageSize);
        var totalCount = await repository.CountLessonsBySectionIdAsync(sectionId);

        return ServiceResult<PagedResult<LessonResponse>>.Ok(new PagedResult<LessonResponse>
        {
            Items = lessons.Select(l => l.ToResponse()).ToList(),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        });
    }

    public async Task<ServiceResult<LessonResponse>> CreateLessonAsync(int sectionId, LessonCreateRequest request)
    {
        var section = await repository.GetSectionByIdAsync(sectionId);
        if (section is null)
        {
            return ServiceResult<LessonResponse>.Fail(StatusCodes.Status404NotFound, "Section not found.");
        }

        if (!TryParseLessonType(request.Type, out var lessonType))
        {
            return ServiceResult<LessonResponse>.Fail(StatusCodes.Status400BadRequest, "Invalid lesson type.");
        }

        var lessons = await repository.GetLessonsBySectionIdAsync(sectionId);
        var order = request.Order ?? (lessons.Count == 0 ? 1 : lessons.Max(l => l.Order) + 1);

        var lesson = new Lesson
        {
            SectionId = sectionId,
            Title = request.Title.Trim(),
            ContentUrl = request.ContentUrl?.Trim(),
            Duration = request.Duration,
            Order = order,
            Type = lessonType,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.AddLessonAsync(lesson);
        await repository.SaveChangesAsync();
        await NormalizeLessonOrderAsync(sectionId);

        var created = await repository.GetLessonByIdAsync(lesson.LessonId);
        return ServiceResult<LessonResponse>.Created(created!.ToResponse(), "Lesson created successfully.");
    }

    public async Task<ServiceResult<LessonResponse>> UpdateLessonAsync(int sectionId, int lessonId, LessonUpdateRequest request)
    {
        var lesson = await repository.GetLessonByIdAsync(lessonId);
        if (lesson is null || lesson.SectionId != sectionId)
        {
            return ServiceResult<LessonResponse>.Fail(StatusCodes.Status404NotFound, "Lesson not found.");
        }

        var section = await repository.GetSectionByIdAsync(sectionId);
        if (section is null)
        {
            return ServiceResult<LessonResponse>.Fail(StatusCodes.Status404NotFound, "Section not found.");
        }

        if (!TryParseLessonType(request.Type, out var lessonType))
        {
            return ServiceResult<LessonResponse>.Fail(StatusCodes.Status400BadRequest, "Invalid lesson type.");
        }

        lesson.Title = request.Title.Trim();
        lesson.ContentUrl = request.ContentUrl?.Trim();
        lesson.Duration = request.Duration;
        lesson.Type = lessonType;
        if (request.Order.HasValue)
        {
            lesson.Order = request.Order.Value;
        }
        lesson.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync();
        await NormalizeLessonOrderAsync(sectionId);

        var updated = await repository.GetLessonByIdAsync(lessonId);
        return ServiceResult<LessonResponse>.Ok(updated!.ToResponse(), "Lesson updated successfully.");
    }

    public async Task<ServiceResult<object>> DeleteLessonAsync(int sectionId, int lessonId)
    {
        var lesson = await repository.GetLessonByIdAsync(lessonId);
        if (lesson is null || lesson.SectionId != sectionId)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Lesson not found.");
        }

        var section = await repository.GetSectionByIdAsync(sectionId);
        if (section is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Section not found.");
        }

        repository.RemoveLesson(lesson);
        await repository.SaveChangesAsync();
        await NormalizeLessonOrderAsync(sectionId);
        return ServiceResult<object>.Ok(null, "Lesson deleted successfully.");
    }

    public async Task<ServiceResult<LessonResponse>> ToggleLessonSoftDeleteAsync(int sectionId, int lessonId)
    {
        var lesson = await repository.GetLessonByIdIncludingDeletedAsync(lessonId);
        if (lesson is null || lesson.SectionId != sectionId)
        {
            return ServiceResult<LessonResponse>.Fail(StatusCodes.Status404NotFound, "Lesson not found.");
        }

        var section = await repository.GetSectionByIdIncludingDeletedAsync(sectionId);
        if (section is null)
        {
            return ServiceResult<LessonResponse>.Fail(StatusCodes.Status404NotFound, "Section not found.");
        }

        if (lesson.IsDeleted)
        {
            lesson.IsDeleted = false;
            lesson.DeletedAt = null;
            lesson.DeletedBy = null;
        }
        else
        {
            lesson.IsDeleted = true;
            lesson.DeletedAt = DateTime.UtcNow;
            lesson.DeletedBy = "Admin";
        }

        lesson.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync();
        return ServiceResult<LessonResponse>.Ok(
            lesson.ToResponse(),
            lesson.IsDeleted ? "Lesson soft deleted successfully." : "Lesson restored successfully.");
    }

    // soft delete list of lessons
    public async Task<ServiceResult<BulkSoftDeleteResponse>> BulkSoftDeleteLessonsAsync(int sectionId, BulkSoftDeleteRequest request)
    {
        var section = await repository.GetSectionByIdAsync(sectionId);
        if (section is null)
        {
            return ServiceResult<BulkSoftDeleteResponse>.Fail(StatusCodes.Status404NotFound, "Section not found.");
        }

        var ids = request.Ids
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        var lessons = await repository.GetLessonsBySectionIdIncludingDeletedAsync(sectionId);
        var matchedLessons = lessons
            .Where(s => ids.Contains(s.LessonId))
            .ToList();
        var processedCount = 0;

        foreach (var lesson in matchedLessons)
        {
            if (request.Restore)
            {
                if (!lesson.IsDeleted)
                {
                    continue;
                }

                lesson.IsDeleted = false;
                lesson.DeletedAt = null;
                lesson.DeletedBy = null;
            }
            else
            {
                if (lesson.IsDeleted)
                {
                    continue;
                }

                lesson.IsDeleted = true;
                lesson.DeletedAt = DateTime.UtcNow;
                lesson.DeletedBy = "Admin";
            }

            lesson.UpdatedAt = DateTime.UtcNow;
            processedCount++;
        }

        if (processedCount > 0)
        {
            await repository.SaveChangesAsync();
        }

        return ServiceResult<BulkSoftDeleteResponse>.Ok(new BulkSoftDeleteResponse
        {
            RequestedCount = request.Ids.Count,
            ProcessedCount = processedCount,
            IgnoredCount = ids.Count - processedCount
        }, request.Restore ? "Lessons restored successfully." : "Lessons soft deleted successfully.");
    }

    public async Task<ServiceResult<PagedResult<LessonResponse>>> GetDeletedLessonsAsync(int sectionId, int page, int pageSize)
    {
        var lessons = await repository.GetLessonsBySectionIdIncludingDeletedAsync(sectionId);
        var deletedLessons = lessons.Where(l => l.IsDeleted).ToList();
        var pagedResult = new PagedResult<LessonResponse>
        {
            Items = deletedLessons.Select(l => l.ToResponse()).ToList(),
            TotalCount = deletedLessons.Count,
            PageNumber = page,
            PageSize = pageSize
        };
        return ServiceResult<PagedResult<LessonResponse>>.Ok(pagedResult);
    }

    public async Task<ServiceResult<IReadOnlyCollection<LessonResponse>>> ReorderLessonsAsync(int sectionId, LessonReorderRequest request)
    {
        var section = await repository.GetSectionByIdAsync(sectionId);
        if (section is null)
        {
            return ServiceResult<IReadOnlyCollection<LessonResponse>>.Fail(StatusCodes.Status404NotFound, "Section not found.");
        }

        var lessons = await repository.GetLessonsBySectionIdAsync(sectionId);
        if (lessons.Count == 0)
        {
            return ServiceResult<IReadOnlyCollection<LessonResponse>>.Fail(StatusCodes.Status404NotFound, "No lessons found in this section.");
        }

        var orderMap = request.Lessons.ToDictionary(l => l.Id, l => l.Order);
        foreach (var lesson in lessons)
        {
            if (orderMap.TryGetValue(lesson.LessonId, out var order))
            {
                lesson.Order = order;
            }
        }

        await repository.SaveChangesAsync();
        await NormalizeLessonOrderAsync(sectionId);

        var updated = await repository.GetLessonsBySectionIdAsync(sectionId);
        return ServiceResult<IReadOnlyCollection<LessonResponse>>.Ok(updated.OrderBy(l => l.Order).Select(l => l.ToResponse()).ToList());
    }

}
