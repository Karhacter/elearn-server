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
    // get by slug
    public async Task<ServiceResult<CourseResponse>> GetBySlugAsync(string slug) {
        var course = await repository.GetBySlugAsync(slug);
        if(course == null) {
            return ServiceResult<CourseResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }
        return ServiceResult<CourseResponse>.Ok(course.ToResponse());
    }

    public async Task<ServiceResult<PagedResult<CourseResponse>>> GetAllAsync(int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Max(1, pageSize);

        var items = await repository.GetPagedAsync(page, pageSize);
        var total = await repository.CountAsync();

        return ServiceResult<PagedResult<CourseResponse>>.Ok(new PagedResult<CourseResponse>
        {
            Items = items.Select(c => c.ToResponse()).ToList(),
            TotalCount = total,
            PageNumber = page,
            PageSize = pageSize
        });
    }

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

    public async Task<ServiceResult<PagedResult<CourseResponse>>> GetDeletedAsync(int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Max(1, pageSize);
        var items = await repository.GetDeletedAsync(page, pageSize);
        var total = await repository.CountAsync();
        return ServiceResult<PagedResult<CourseResponse>>.Ok(new PagedResult<CourseResponse>
        {
            Items = items.Select(c => c.ToResponse()).ToList(),
            TotalCount = total,
            PageNumber = page,
            PageSize = pageSize
        });
    }

    public async Task<ServiceResult<PagedResult<CourseClientResponse>>> GetClientPagedAsync(int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Max(1, pageSize);

        var items = await repository.GetClientPagedAsync(pageNumber, pageSize);
        var total = await repository.CountClientAsync();

        return ServiceResult<PagedResult<CourseClientResponse>>.Ok(new PagedResult<CourseClientResponse>
        {
            Items = items.Select(c => c.ToClientResponse()).ToList(),
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    // public async Task<ServiceResult<CourseResponse>> GetByIdAsync(int id)
    // {
    //     var course = await repository.GetByIdAsync(id);
    //     return course is null
    //         ? ServiceResult<CourseResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.")
    //         : ServiceResult<CourseResponse>.Ok(course.ToResponse());
    // }

    public async Task<ServiceResult<CourseResponse>> CreateAsync(CourseCreateRequest request)
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
            Status = request.Status,
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

    public async Task<ServiceResult<CourseResponse>> UpdateAsync(int id, CourseCreateRequest request)
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
        course.Status = request.Status;
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

    public async Task<ServiceResult<CourseResponse>> ToggleSoftDeleteAsync(int id)
    {
        var course = await repository.GetByIdIncludingDeletedAsync(id);
        if (course is null)
        {
            return ServiceResult<CourseResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        if (course.IsDeleted)
        {
            course.IsDeleted = false;
            course.DeletedAt = null;
            course.DeletedBy = null;
        }
        else
        {
            course.IsDeleted = true;
            course.DeletedAt = DateTime.UtcNow;
            course.DeletedBy = "Admin";
        }

        course.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync();
        return ServiceResult<CourseResponse>.Ok(
            course.ToResponse(),
            course.IsDeleted ? "Course soft deleted successfully." : "Course restored successfully.");
    }

    public async Task<ServiceResult<BulkSoftDeleteResponse>> BulkSoftDeleteAsync(BulkSoftDeleteRequest request)
    {
        var ids = request.Ids
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        var processedCount = 0;
        foreach (var id in ids)
        {
            var course = await repository.GetByIdIncludingDeletedAsync(id);
            if (course is null)
            {
                continue;
            }

            if (request.Restore)
            {
                if (!course.IsDeleted)
                {
                    continue;
                }

                course.IsDeleted = false;
                course.DeletedAt = null;
                course.DeletedBy = null;
            }
            else
            {
                if (course.IsDeleted)
                {
                    continue;
                }

                course.IsDeleted = true;
                course.DeletedAt = DateTime.UtcNow;
                course.DeletedBy = "Admin";
            }

            course.UpdatedAt = DateTime.UtcNow;
            processedCount++;
        }

        if (processedCount > 0)
        {
            await repository.SaveChangesAsync();
        }

        return ServiceResult<BulkSoftDeleteResponse>.Ok(new BulkSoftDeleteResponse
        {
            RequestedCount = ids.Count,
            ProcessedCount = processedCount,
            IgnoredCount = ids.Count - processedCount
        }, request.Restore ? "Courses restored successfully." : "Courses soft deleted successfully.");
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

        return await UpdateImageAsync(id, await fileStorageService.SaveFileAsync(imageFile, "course", cancellationToken));
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

}
