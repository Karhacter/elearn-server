using System.Text.RegularExpressions;
using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Mappings;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Domain.Enums;
using elearn_server.Infrastructure.Persistence.Repositories;
namespace elearn_server.Infrastructure.Services.Core.Courses;

public class CourseService(ICourseRepository repository, IFileStorageService fileStorageService) : ICourseService
{
    public async Task<ServiceResult<IReadOnlyCollection<CourseResponse>>> GetAllAsync() =>
        ServiceResult<IReadOnlyCollection<CourseResponse>>.Ok((await repository.GetAllAsync()).Select(c => c.ToResponse()).ToList());

    public async Task<ServiceResult<PagedResult<CourseResponse>>> GetPagedAsync(int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Max(1, pageSize);
        var items = await repository.GetPagedAsync(pageNumber, pageSize);
        var total = await repository.CountAsync();
        return ServiceResult<PagedResult<CourseResponse>>.Ok(new PagedResult<CourseResponse>
        {
            Items = items.Select(c => c.ToResponse()).ToList(),
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    public async Task<ServiceResult<CourseResponse>> GetByIdAsync(int id)
    {
        var course = await repository.GetByIdAsync(id);
        return course is null
            ? ServiceResult<CourseResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.")
            : ServiceResult<CourseResponse>.Ok(course.ToResponse());
    }

    public async Task<ServiceResult<CourseResponse>> CreateAsync(CourseUpsertRequest request)
    {
        if (await repository.GetByTitleAsync(request.Title.Trim()) is not null)
        {
            return ServiceResult<CourseResponse>.Fail(StatusCodes.Status409Conflict, "Tên khóa học đã tồn tại.");
        }
        if (!await repository.UserExistsAsync(request.InstructorId))
        {
            return ServiceResult<CourseResponse>.Fail(StatusCodes.Status404NotFound, "Instructor not found.");
        }
        if (!await repository.CategoryExistsAsync(request.GenreId))
        {
            return ServiceResult<CourseResponse>.Fail(StatusCodes.Status404NotFound, "Category not found.");
        }

        var slug = await GenerateUniqueSlugAsync(request.Slug, request.Title);
        var course = new Course
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            Discount = request.Discount,
            GenreId = request.GenreId,
            Duration = request.Duration,
            Thumbnail = request.Thumbnail,
            Image = request.Image,
            InstructorId = request.InstructorId,
            Slug = slug,
            IsSequential = request.IsSequential,
            Status = CourseStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "system"
        };

        course.LearningOutcomes = BuildOutcomeList(request.LearningOutcomes, course);
        course.Requirements = BuildRequirementList(request.Requirements, course);
        course.TargetAudiences = BuildTargetAudienceList(request.TargetAudiences, course);

        await repository.AddAsync(course);
        await repository.SaveChangesAsync();
        var created = await repository.GetByIdAsync(course.CourseId);
        return ServiceResult<CourseResponse>.Created(created!.ToResponse(), "Course created successfully.");
    }

    public async Task<ServiceResult<CourseResponse>> UpdateAsync(int id, CourseUpsertRequest request)
    {
        var course = await repository.GetByIdWithStructureAsync(id);
        if (course is null)
        {
            return ServiceResult<CourseResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        course.Title = request.Title.Trim();
        course.Description = request.Description.Trim();
        course.Duration = request.Duration;
        course.Thumbnail = request.Thumbnail;
        course.Price = request.Price;
        course.Discount = request.Discount;
        course.GenreId = request.GenreId;
        course.InstructorId = request.InstructorId;
        course.Image = request.Image;
        course.Slug = await ResolveSlugAsync(course.Slug, request.Slug, request.Title, id);
        course.IsSequential = request.IsSequential;
        course.UpdatedAt = DateTime.UtcNow;

        ReplaceCourseList(course.LearningOutcomes, request.LearningOutcomes, (content, order) => new LearningOutcome
        {
            Content = content,
            Order = order,
            CourseId = course.CourseId
        });
        ReplaceCourseList(course.Requirements, request.Requirements, (content, order) => new CourseRequirement
        {
            Content = content,
            Order = order,
            CourseId = course.CourseId
        });
        ReplaceCourseList(course.TargetAudiences, request.TargetAudiences, (content, order) => new CourseTargetAudience
        {
            Content = content,
            Order = order,
            CourseId = course.CourseId
        });

        await repository.SaveChangesAsync();
        var updated = await repository.GetByIdAsync(id);
        return ServiceResult<CourseResponse>.Ok(updated!.ToResponse(), "Course updated successfully.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var course = await repository.GetByIdAsync(id);
        if (course is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        repository.Remove(course);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Course deleted successfully.");
    }

    public async Task<ServiceResult<IReadOnlyCollection<CourseResponse>>> GetByCategoryIdAsync(int categoryId)
    {
        var courses = await repository.GetByCategoryIdAsync(categoryId);
        return courses.Count == 0
            ? ServiceResult<IReadOnlyCollection<CourseResponse>>.Fail(StatusCodes.Status404NotFound, $"No courses found for category ID {categoryId}.")
            : ServiceResult<IReadOnlyCollection<CourseResponse>>.Ok(courses.Select(c => c.ToResponse()).ToList());
    }

    public async Task<ServiceResult<ImageUploadResponse>> UpdateImageAsync(int id, string imageUrl)
    {
        var course = await repository.GetByIdAsync(id);
        if (course is null)
        {
            return ServiceResult<ImageUploadResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        course.Image = imageUrl;
        await repository.SaveChangesAsync();
        return ServiceResult<ImageUploadResponse>.Ok(new ImageUploadResponse { ImageUrl = imageUrl }, "Course image updated successfully.");
    }

    public async Task<ServiceResult<ImageUploadResponse>> UploadImageAsync(int id, IFormFile imageFile, CancellationToken cancellationToken)
    {
        if (imageFile is null || imageFile.Length == 0)
        {
            return ServiceResult<ImageUploadResponse>.Fail(StatusCodes.Status400BadRequest, "No image file provided.");
        }

        return await UpdateImageAsync(id, await fileStorageService.SaveImageAsync(imageFile, cancellationToken));
    }

    public async Task<ServiceResult<IReadOnlyCollection<CourseResponse>>> SearchAsync(string? keyword, int? genreId, int? instructorId) =>
        ServiceResult<IReadOnlyCollection<CourseResponse>>.Ok((await repository.SearchAsync(keyword, genreId, instructorId)).Select(c => c.ToResponse()).ToList());

    public async Task<ServiceResult<CoursePreviewResponse>> PreviewAsync(int courseId)
    {
        var course = await repository.GetByIdWithStructureAsync(courseId);
        return course is null
            ? ServiceResult<CoursePreviewResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.")
            : ServiceResult<CoursePreviewResponse>.Ok(course.ToPreviewResponse());
    }

    public async Task<ServiceResult<CourseResponse>> PublishAsync(int courseId, bool isAdmin)
    {
        var course = await repository.GetByIdAsync(courseId);
        if (course is null)
        {
            return ServiceResult<CourseResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        course.Status = isAdmin ? CourseStatus.Published : CourseStatus.PendingReview;
        course.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync();
        var updated = await repository.GetByIdAsync(courseId);
        return ServiceResult<CourseResponse>.Ok(updated!.ToResponse(), isAdmin ? "Course published successfully." : "Course submitted for review.");
    }

    public async Task<ServiceResult<CourseResponse>> UnpublishAsync(int courseId)
    {
        var course = await repository.GetByIdAsync(courseId);
        if (course is null)
        {
            return ServiceResult<CourseResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        course.Status = CourseStatus.Draft;
        course.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync();
        var updated = await repository.GetByIdAsync(courseId);
        return ServiceResult<CourseResponse>.Ok(updated!.ToResponse(), "Course moved to draft.");
    }

    public async Task<ServiceResult<SectionResponse>> CreateSectionAsync(int courseId, SectionCreateRequest request)
    {
        var course = await repository.GetByIdAsync(courseId);
        if (course is null)
        {
            return ServiceResult<SectionResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        var sections = await repository.GetSectionsByCourseIdAsync(courseId);
        var order = request.Order ?? (sections.Count == 0 ? 1 : sections.Max(s => s.Order) + 1);
        var section = new CourseSection
        {
            CourseId = courseId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Order = order,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.AddSectionAsync(section);
        await repository.SaveChangesAsync();
        await NormalizeSectionOrderAsync(courseId);

        var created = await repository.GetSectionByIdAsync(section.SectionId);
        return ServiceResult<SectionResponse>.Created(created!.ToResponse(), "Section created successfully.");
    }

    public async Task<ServiceResult<SectionResponse>> UpdateSectionAsync(int courseId, int sectionId, SectionUpdateRequest request)
    {
        var section = await repository.GetSectionByIdAsync(sectionId);
        if (section is null || section.CourseId != courseId)
        {
            return ServiceResult<SectionResponse>.Fail(StatusCodes.Status404NotFound, "Section not found.");
        }

        section.Title = request.Title.Trim();
        section.Description = request.Description?.Trim();
        if (request.Order.HasValue)
        {
            section.Order = request.Order.Value;
        }

        section.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync();
        await NormalizeSectionOrderAsync(courseId);

        var updated = await repository.GetSectionByIdAsync(sectionId);
        return ServiceResult<SectionResponse>.Ok(updated!.ToResponse(), "Section updated successfully.");
    }

    public async Task<ServiceResult<object>> DeleteSectionAsync(int courseId, int sectionId)
    {
        var section = await repository.GetSectionByIdAsync(sectionId);
        if (section is null || section.CourseId != courseId)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Section not found.");
        }

        repository.RemoveSection(section);
        await repository.SaveChangesAsync();
        await NormalizeSectionOrderAsync(courseId);
        return ServiceResult<object>.Ok(null, "Section deleted successfully.");
    }

    public async Task<ServiceResult<IReadOnlyCollection<SectionResponse>>> ReorderSectionsAsync(int courseId, SectionReorderRequest request)
    {
        var sections = await repository.GetSectionsByCourseIdAsync(courseId);
        if (sections.Count == 0)
        {
            return ServiceResult<IReadOnlyCollection<SectionResponse>>.Fail(StatusCodes.Status404NotFound, "No sections found for this course.");
        }

        var orderMap = request.Sections.ToDictionary(s => s.Id, s => s.Order);
        foreach (var section in sections)
        {
            if (orderMap.TryGetValue(section.SectionId, out var order))
            {
                section.Order = order;
            }
        }

        await repository.SaveChangesAsync();
        await NormalizeSectionOrderAsync(courseId);

        var updated = await repository.GetSectionsByCourseIdAsync(courseId);
        return ServiceResult<IReadOnlyCollection<SectionResponse>>.Ok(updated.OrderBy(s => s.Order).Select(s => s.ToResponse()).ToList());
    }

    public async Task<ServiceResult<LessonResponse>> CreateLessonAsync(int courseId, int sectionId, LessonCreateRequest request)
    {
        var section = await repository.GetSectionByIdAsync(sectionId);
        if (section is null || section.CourseId != courseId)
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
            CourseId = courseId,
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

    public async Task<ServiceResult<LessonResponse>> UpdateLessonAsync(int courseId, int sectionId, int lessonId, LessonUpdateRequest request)
    {
        var lesson = await repository.GetLessonByIdAsync(lessonId);
        if (lesson is null || lesson.CourseId != courseId || lesson.SectionId != sectionId)
        {
            return ServiceResult<LessonResponse>.Fail(StatusCodes.Status404NotFound, "Lesson not found.");
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

    public async Task<ServiceResult<object>> DeleteLessonAsync(int courseId, int sectionId, int lessonId)
    {
        var lesson = await repository.GetLessonByIdAsync(lessonId);
        if (lesson is null || lesson.CourseId != courseId || lesson.SectionId != sectionId)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Lesson not found.");
        }

        repository.RemoveLesson(lesson);
        await repository.SaveChangesAsync();
        await NormalizeLessonOrderAsync(sectionId);
        return ServiceResult<object>.Ok(null, "Lesson deleted successfully.");
    }

    public async Task<ServiceResult<IReadOnlyCollection<LessonResponse>>> ReorderLessonsAsync(int courseId, int sectionId, LessonReorderRequest request)
    {
        var section = await repository.GetSectionByIdAsync(sectionId);
        if (section is null || section.CourseId != courseId)
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

    private async Task NormalizeSectionOrderAsync(int courseId)
    {
        var sections = await repository.GetSectionsByCourseIdAsync(courseId);
        var ordered = sections.OrderBy(s => s.Order).ToList();
        for (var i = 0; i < ordered.Count; i++)
        {
            ordered[i].Order = i + 1;
        }
        await repository.SaveChangesAsync();
    }

    private async Task NormalizeLessonOrderAsync(int sectionId)
    {
        var lessons = await repository.GetLessonsBySectionIdAsync(sectionId);
        var ordered = lessons.OrderBy(l => l.Order).ToList();
        for (var i = 0; i < ordered.Count; i++)
        {
            ordered[i].Order = i + 1;
        }
        await repository.SaveChangesAsync();
    }

    private static bool TryParseLessonType(string? value, out LessonType lessonType)
    {
        lessonType = LessonType.Video;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim();
        if (normalized.Equals("downloadable resource", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("downloadable-resource", StringComparison.OrdinalIgnoreCase))
        {
            lessonType = LessonType.Resource;
            return true;
        }

        return Enum.TryParse(normalized, true, out lessonType);
    }

    private static List<LearningOutcome> BuildOutcomeList(IEnumerable<string> items, Course course)
    {
        var result = new List<LearningOutcome>();
        var index = 1;
        foreach (var item in items.Where(i => !string.IsNullOrWhiteSpace(i)))
        {
            result.Add(new LearningOutcome
            {
                Course = course,
                Content = item.Trim(),
                Order = index++
            });
        }
        return result;
    }

    private static List<CourseRequirement> BuildRequirementList(IEnumerable<string> items, Course course)
    {
        var result = new List<CourseRequirement>();
        var index = 1;
        foreach (var item in items.Where(i => !string.IsNullOrWhiteSpace(i)))
        {
            result.Add(new CourseRequirement
            {
                Course = course,
                Content = item.Trim(),
                Order = index++
            });
        }
        return result;
    }

    private static List<CourseTargetAudience> BuildTargetAudienceList(IEnumerable<string> items, Course course)
    {
        var result = new List<CourseTargetAudience>();
        var index = 1;
        foreach (var item in items.Where(i => !string.IsNullOrWhiteSpace(i)))
        {
            result.Add(new CourseTargetAudience
            {
                Course = course,
                Content = item.Trim(),
                Order = index++
            });
        }
        return result;
    }

    private static void ReplaceCourseList<T>(ICollection<T>? existing, IEnumerable<string> items, Func<string, int, T> factory)
        where T : class
    {
        existing ??= new List<T>();
        existing.Clear();
        var index = 1;
        foreach (var item in items.Where(i => !string.IsNullOrWhiteSpace(i)))
        {
            existing.Add(factory(item.Trim(), index++));
        }
    }

    private async Task<string?> ResolveSlugAsync(string? existingSlug, string? requestedSlug, string title, int courseId)
    {
        if (!string.IsNullOrWhiteSpace(requestedSlug))
        {
            return await GenerateUniqueSlugAsync(requestedSlug, title, courseId);
        }

        if (!string.IsNullOrWhiteSpace(existingSlug))
        {
            return existingSlug;
        }

        return await GenerateUniqueSlugAsync(null, title, courseId);
    }

    private async Task<string?> GenerateUniqueSlugAsync(string? requestedSlug, string title, int? ignoreCourseId = null)
    {
        var baseSlug = NormalizeSlug(string.IsNullOrWhiteSpace(requestedSlug) ? title : requestedSlug);
        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            return null;
        }

        var slug = baseSlug;
        var suffix = 1;
        while (true)
        {
            var existing = await repository.GetBySlugAsync(slug);
            if (existing is null || (ignoreCourseId.HasValue && existing.CourseId == ignoreCourseId.Value))
            {
                return slug;
            }

            slug = $"{baseSlug}-{suffix++}";
        }
    }

    private static string NormalizeSlug(string value)
    {
        var slug = value.Trim().ToLowerInvariant();
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", string.Empty);
        slug = Regex.Replace(slug, @"\-{2,}", "-").Trim('-');
        return slug;
    }
}
