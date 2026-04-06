using elearn_server.Application.Common;
using elearn_server.Application.DTOs;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using Microsoft.AspNetCore.Http;

namespace elearn_server.Application.Interfaces;

public interface IUserService
{
    Task<ServiceResult<PagedResult<AuthenticatedUserResponse>>> GetAllAsync(int page, int pageSize);
    Task<ServiceResult<IReadOnlyCollection<AuthenticatedUserResponse>>> GetDeletedAsync();
    Task<ServiceResult<AuthenticatedUserResponse>> GetByIdAsync(int id);
    Task<ServiceResult<AuthenticatedUserResponse>> CreateAsync(UserCreateDTO request);
    Task<ServiceResult<AuthenticatedUserResponse>> UpdateAsync(int id, UserUpdateRequest request);
    Task<ServiceResult<object>> DeleteAsync(int id);
    Task<ServiceResult<AuthenticatedUserResponse>> ToggleSoftDeleteAsync(int id);
    Task<ServiceResult<BulkSoftDeleteResponse>> BulkSoftDeleteAsync(BulkSoftDeleteRequest request);
    Task<ServiceResult<ImageUploadResponse>> UpdateImageAsync(int id, string imageUrl);
    Task<ServiceResult<ImageUploadResponse>> UploadImageAsync(int id, IFormFile imageFile, CancellationToken cancellationToken);
}
