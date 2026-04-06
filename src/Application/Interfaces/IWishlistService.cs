using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface IWishlistService
{
    Task<ServiceResult<IReadOnlyCollection<WishlistResponse>>> GetByUserIdAsync(int userId);
    Task<ServiceResult<WishlistResponse>> CreateAsync(WishlistCreateRequest request);
    Task<ServiceResult<object>> DeleteAsync(int id);
}