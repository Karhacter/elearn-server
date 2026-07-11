using System.Text.RegularExpressions;
using elearn_server.Application.Common;
using elearn_server.Application.Mappings;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Domain.Enums;
using elearn_server.Application.Interfaces;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;
namespace elearn_server.Infrastructure.Services.Core.Courses;

public partial class CourseService(ICourseRepository repository, IFileStorageService fileStorageService) : ICourseService
{
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

    // Others
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

    public async Task<ServiceResult<CourseResponse>> GetByIdAsync(int id)
    {
        var course = await repository.GetByIdAsync(id);
        return course is null
            ? ServiceResult<CourseResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.")
            : ServiceResult<CourseResponse>.Ok(course.ToResponse());
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
