using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Application.Mappings;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence.Repositories;
using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
namespace elearn_server.Infrastructure.Services.Commerce;

public class CommentService(ICommentRepository repository) : ICommentService
{
    public async Task<ServiceResult<IReadOnlyCollection<CommentResponse>>> GetAllAsync() =>
        ServiceResult<IReadOnlyCollection<CommentResponse>>.Ok((await repository.GetAllAsync()).Select(c => c.ToResponse()).ToList());

    public async Task<ServiceResult<CommentResponse>> CreateAsync(CommentCreateRequest request)
    {
        if (await repository.GetUserAsync(request.UserId) is null)
        {
            return ServiceResult<CommentResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }
        if (await repository.GetCourseAsync(request.CourseId) is null)
        {
            return ServiceResult<CommentResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        var comment = new Comment
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            Content = request.Content.Trim(),
            CommentDate = DateTime.UtcNow
        };

        await repository.AddAsync(comment);
        await repository.SaveChangesAsync();
        return ServiceResult<CommentResponse>.Created(comment.ToResponse(), "Comment created successfully.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var comment = await repository.GetByIdAsync(id);
        if (comment is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Comment not found.");
        }

        repository.Remove(comment);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Comment deleted successfully.");
    }
}
