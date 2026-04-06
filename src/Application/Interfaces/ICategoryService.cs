using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult<IReadOnlyCollection<CategoryResponse>>> GetAllAsync();
    Task<ServiceResult<CategoryResponse>> GetByIdAsync(int id);
    Task<ServiceResult<CategoryResponse>> CreateAsync(CategoryUpsertRequest request);
    Task<ServiceResult<CategoryResponse>> UpdateAsync(int id, CategoryUpsertRequest request);
    Task<ServiceResult<object>> DeleteAsync(int id);
}