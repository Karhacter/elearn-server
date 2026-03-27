using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Application.Mappings;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence.Repositories;
using elearn_server.Application.Common;

namespace elearn_server.Application.Interfaces;

public class WishlistService(IWishlistRepository repository) : IWishlistService
{
    public async Task<ServiceResult<IReadOnlyCollection<WishlistResponse>>> GetByUserIdAsync(int userId) =>
        ServiceResult<IReadOnlyCollection<WishlistResponse>>.Ok((await repository.GetByUserIdAsync(userId)).Select(w => w.ToResponse()).ToList());

    public async Task<ServiceResult<WishlistResponse>> CreateAsync(WishlistCreateRequest request)
    {
        if (!await repository.UserExistsAsync(request.UserId))
        {
            return ServiceResult<WishlistResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }
        if (await repository.GetCourseAsync(request.CourseId) is null)
        {
            return ServiceResult<WishlistResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }
        if (await repository.GetByUserAndCourseAsync(request.UserId, request.CourseId) is not null)
        {
            return ServiceResult<WishlistResponse>.Fail(StatusCodes.Status409Conflict, "Course is already in wishlist.");
        }

        var wishlist = new Wishlist
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            AddedDate = DateTime.UtcNow
        };

        await repository.AddAsync(wishlist);
        await repository.SaveChangesAsync();
        var created = await repository.GetByIdAsync(wishlist.Id);
        return ServiceResult<WishlistResponse>.Created(created!.ToResponse(), "Wishlist item created successfully.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var wishlist = await repository.GetByIdAsync(id);
        if (wishlist is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Wishlist item not found.");
        }

        repository.Remove(wishlist);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Wishlist item removed successfully.");
    }
}
