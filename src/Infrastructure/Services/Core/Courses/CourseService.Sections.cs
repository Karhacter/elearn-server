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
    // For Sections
    public async Task<ServiceResult<IReadOnlyCollection<SectionResponse>>> GetSectionsAsync(int courseId)
    {
        var sections = await repository.GetSectionsByCourseIdAsync(courseId);
        return ServiceResult<IReadOnlyCollection<SectionResponse>>.Ok(sections.Select(s => s.ToResponse()).ToList());
    }

    public async Task<ServiceResult<IReadOnlyCollection<SectionResponse>>> GetDeletedSectionsAsync(int courseId)
    {
        var sections = await repository.GetDeletedSectionsByCourseIdAsync(courseId);
        return ServiceResult<IReadOnlyCollection<SectionResponse>>.Ok(sections.Select(s => s.ToResponse()).ToList());
    }


    public async Task<ServiceResult<BulkSoftDeleteResponse>> BulkSoftDeleteSectionsAsync(int courseId, BulkSoftDeleteRequest request)
    {
        var course = await repository.GetByIdAsync(courseId);
        if (course is null)
        {
            return ServiceResult<BulkSoftDeleteResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        var ids = request.Ids
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        var sections = await repository.GetSectionsByCourseIdIncludingDeletedAsync(courseId);
        var matchedSections = sections
            .Where(s => ids.Contains(s.SectionId))
            .ToList();
        var processedCount = 0;

        foreach (var section in matchedSections)
        {
            if (request.Restore)
            {
                if (!section.IsDeleted)
                {
                    continue;
                }

                section.IsDeleted = false;
                section.DeletedAt = null;
                section.DeletedBy = null;
            }
            else
            {
                if (section.IsDeleted)
                {
                    continue;
                }

                section.IsDeleted = true;
                section.DeletedAt = DateTime.UtcNow;
                section.DeletedBy = "Admin";
            }

            section.UpdatedAt = DateTime.UtcNow;
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
        }, request.Restore ? "Sections restored successfully." : "Sections soft deleted successfully.");
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
        return ServiceResult<object>.Ok(null, "Section soft deleted successfully.");
    }

    public async Task<ServiceResult<SectionResponse>> ToggleSectionSoftDeleteAsync(int courseId, int sectionId)
    {
        var section = await repository.GetSectionByIdIncludingDeletedAsync(sectionId);
        if (section is null || section.CourseId != courseId)
        {
            return ServiceResult<SectionResponse>.Fail(StatusCodes.Status404NotFound, "Section not found.");
        }

        if (section.IsDeleted)
        {
            section.IsDeleted = false;
            section.DeletedAt = null;
            section.DeletedBy = null;
        }
        else
        {
            section.IsDeleted = true;
            section.DeletedAt = DateTime.UtcNow;
            section.DeletedBy = "Admin";
        }

        section.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync();
        await NormalizeSectionOrderAsync(courseId);

        var updated = await repository.GetSectionByIdIncludingDeletedAsync(sectionId);
        return ServiceResult<SectionResponse>.Ok(
            updated!.ToResponse(),
            section.IsDeleted ? "Section soft deleted successfully." : "Section restored successfully.");
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

}
