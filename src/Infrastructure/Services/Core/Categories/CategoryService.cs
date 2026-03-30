using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Mappings;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence.Repositories;

namespace elearn_server.Infrastructure.Services.Core.Categories;

public class CategoryService(ICategoryRepository repository) : ICategoryService
{
    public async Task<ServiceResult<IReadOnlyCollection<CategoryResponse>>> GetAllAsync() =>
        ServiceResult<IReadOnlyCollection<CategoryResponse>>.Ok((await repository.GetAllAsync()).Select(c => c.ToResponse()).ToList());

    public async Task<ServiceResult<CategoryResponse>> GetByIdAsync(int id)
    {
        var category = await repository.GetByIdAsync(id);
        return category is null
            ? ServiceResult<CategoryResponse>.Fail(StatusCodes.Status404NotFound, "Category not found.")
            : ServiceResult<CategoryResponse>.Ok(category.ToResponse());
    }

    public async Task<ServiceResult<CategoryResponse>> CreateAsync(CategoryUpsertRequest request)
    {
        if (await repository.GetByNameAsync(request.Name.Trim()) is not null)
        {
            return ServiceResult<CategoryResponse>.Fail(StatusCodes.Status409Conflict, "Tên thể loại đã tồn tại.");
        }

        var category = new Category
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "system"
        };

        await repository.AddAsync(category);
        await repository.SaveChangesAsync();
        return ServiceResult<CategoryResponse>.Created(category.ToResponse(), "Category created successfully.");
    }

    public async Task<ServiceResult<CategoryResponse>> UpdateAsync(int id, CategoryUpsertRequest request)
    {
        var category = await repository.GetByIdAsync(id);
        if (category is null)
        {
            return ServiceResult<CategoryResponse>.Fail(StatusCodes.Status404NotFound, "Category not found.");
        }

        category.Name = request.Name.Trim();
        category.Description = request.Description?.Trim();
        category.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync();
        return ServiceResult<CategoryResponse>.Ok(category.ToResponse(), "Category updated successfully.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var category = await repository.GetByIdAsync(id);
        if (category is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Category not found.");
        }

        repository.Remove(category);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Category deleted successfully.");
    }
}
