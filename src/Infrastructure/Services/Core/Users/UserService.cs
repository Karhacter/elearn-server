using elearn_server.Application.Common;
using elearn_server.Application.DTOs;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Mappings;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Domain.Enums;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;
using Microsoft.AspNetCore.Identity;

namespace elearn_server.Infrastructure.Services.Core.Users;

public class UserService(IUserRepository repository, IFileStorageService fileStorageService) : IUserService
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public async Task<ServiceResult<PagedResult<AuthenticatedUserResponse>>> GetAllAsync(int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Max(1, pageSize);

        var users = await repository.GetPagedAsync(page, pageSize);
        var totalCount = await repository.CountAsync();

        return ServiceResult<PagedResult<AuthenticatedUserResponse>>.Ok(new PagedResult<AuthenticatedUserResponse>
        {
            Items = users.Select(u => u.ToAuthenticatedUserResponse()).ToList(),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        });
    }

    public async Task<ServiceResult<PagedResult<AuthenticatedUserResponse>>> GetAllInstructorsAsync(int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Max(1, pageSize);

        var users = await repository.GetPagedInstructorsAsync(page, pageSize);
        var totalCount = await repository.CountInstructorsAsync();

        return ServiceResult<PagedResult<AuthenticatedUserResponse>>.Ok(new PagedResult<AuthenticatedUserResponse>
        {
            Items = users.Select(u => u.ToAuthenticatedUserResponse()).ToList(),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        });
    }
    // Get deleted Instructor
    public async Task<ServiceResult<IReadOnlyCollection<AuthenticatedUserResponse>>> GetDeletedInstructorsAsync(int page, int pageSize) =>
        ServiceResult<IReadOnlyCollection<AuthenticatedUserResponse>>.Ok((await repository.GetAllDeletedInstructorsAsync()).Select(u => u.ToAuthenticatedUserResponse()).ToList());


    public async Task<ServiceResult<IReadOnlyCollection<AuthenticatedUserResponse>>> GetDeletedAsync() =>
        ServiceResult<IReadOnlyCollection<AuthenticatedUserResponse>>.Ok((await repository.GetAllDeletedAsync()).Select(u => u.ToAuthenticatedUserResponse()).ToList());

    public async Task<ServiceResult<AuthenticatedUserResponse>> GetByIdAsync(int id)
    {
        var user = await repository.GetByIdIncludingDeletedAsync(id);
        return user is null
            ? ServiceResult<AuthenticatedUserResponse>.Fail(StatusCodes.Status404NotFound, "User not found.")
            : ServiceResult<AuthenticatedUserResponse>.Ok(user.ToAuthenticatedUserResponse());
    }

    public async Task<ServiceResult<AuthenticatedUserResponse>> CreateAsync(UserCreateDTO request)
    {
        if (await repository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant()) is not null)
        {
            return ServiceResult<AuthenticatedUserResponse>.Fail(StatusCodes.Status409Conflict, "Email is already registered.");
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Role = request.Role.Trim(),
            ProfilePicture = string.IsNullOrWhiteSpace(request.ProfilePicture) ? null : request.ProfilePicture.Trim(),
            Gender = ParseGender(request.Gender),
            Birthday = request.Birthday,
            CountryCode = string.IsNullOrWhiteSpace(request.CountryCode) ? null : request.CountryCode.Trim(),
            CountryName = string.IsNullOrWhiteSpace(request.CountryName) ? null : request.CountryName.Trim(),
            CityCode = string.IsNullOrWhiteSpace(request.CityCode) ? null : request.CityCode.Trim(),
            CityName = string.IsNullOrWhiteSpace(request.CityName) ? null : request.CityName.Trim(),
            Street = string.IsNullOrWhiteSpace(request.Street) ? null : request.Street.Trim(),
            IsEmailVerified = true,
            EmailVerifiedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "system"
        };

        user.Password = _passwordHasher.HashPassword(user, request.Password.Trim());

        await repository.AddAsync(user);
        await repository.SaveChangesAsync();
        return ServiceResult<AuthenticatedUserResponse>.Created(user.ToAuthenticatedUserResponse(), "User created successfully.");
    }

    public async Task<ServiceResult<AuthenticatedUserResponse>> UpdateAsync(int id, UserUpdateRequest request)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null)
        {
            return ServiceResult<AuthenticatedUserResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }

        user.FullName = request.FullName.Trim();
        user.Email = request.Email.Trim().ToLowerInvariant();
        user.PhoneNumber = request.PhoneNumber.Trim();
        user.Role = request.Role.Trim();
        user.ProfilePicture = request.ProfilePicture;
        user.Gender = ParseGender(request.Gender);
        user.Birthday = request.Birthday;
        user.CountryCode = string.IsNullOrWhiteSpace(request.CountryCode) ? null : request.CountryCode.Trim();
        user.CountryName = string.IsNullOrWhiteSpace(request.CountryName) ? null : request.CountryName.Trim();
        user.CityCode = string.IsNullOrWhiteSpace(request.CityCode) ? null : request.CityCode.Trim();
        user.CityName = string.IsNullOrWhiteSpace(request.CityName) ? null : request.CityName.Trim();
        user.Street = string.IsNullOrWhiteSpace(request.Street) ? null : request.Street.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync();
        return ServiceResult<AuthenticatedUserResponse>.Ok(user.ToAuthenticatedUserResponse(), "User updated successfully.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var user = await repository.GetByIdIncludingDeletedAsync(id);
        if (user is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }

        if (user.IsDeleted)
        {
            repository.RemovePermanently(user);
            await repository.SaveChangesAsync();
            return ServiceResult<object>.Ok(null, "User permanently deleted successfully.");
        }

        repository.Remove(user);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "User soft deleted successfully.");
    }

    public async Task<ServiceResult<AuthenticatedUserResponse>> ToggleSoftDeleteAsync(int id)
    {
        var user = await repository.GetByIdIncludingDeletedAsync(id);
        if (user is null)
        {
            return ServiceResult<AuthenticatedUserResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }

        if (user.IsDeleted)
        {
            user.IsDeleted = false;
            user.DeletedAt = null;
            user.DeletedBy = null;
        }
        else
        {
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.DeletedBy = "Admin";
        }

        user.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync();
        return ServiceResult<AuthenticatedUserResponse>.Ok(
            user.ToAuthenticatedUserResponse(),
            user.IsDeleted ? "User soft deleted successfully." : "User restored successfully.");
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
            var user = await repository.GetByIdIncludingDeletedAsync(id);
            if (user is null)
            {
                continue;
            }

            if (request.Restore)
            {
                if (!user.IsDeleted)
                {
                    continue;
                }

                user.IsDeleted = false;
                user.DeletedAt = null;
                user.DeletedBy = null;
            }
            else
            {
                if (user.IsDeleted)
                {
                    continue;
                }

                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;
                user.DeletedBy = "Admin";
            }

            user.UpdatedAt = DateTime.UtcNow;
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
        }, request.Restore ? "Users restored successfully." : "Users soft deleted successfully.");
    }

    public async Task<ServiceResult<ImageUploadResponse>> UpdateImageAsync(int id, string imageUrl)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null)
        {
            return ServiceResult<ImageUploadResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }

        user.ProfilePicture = imageUrl;
        await repository.SaveChangesAsync();
        return ServiceResult<ImageUploadResponse>.Ok(new ImageUploadResponse { ImageUrl = imageUrl }, "User image updated successfully.");
    }

    public async Task<ServiceResult<ImageUploadResponse>> UploadImageAsync(int id, IFormFile imageFile, CancellationToken cancellationToken)
    {
        if (imageFile is null || imageFile.Length == 0)
        {
            return ServiceResult<ImageUploadResponse>.Fail(StatusCodes.Status400BadRequest, "No image file provided.");
        }

        return await UpdateImageAsync(id, await fileStorageService.SaveFileAsync(imageFile, "user", cancellationToken));
    }

    private static GenderType? ParseGender(string? gender) =>
        string.IsNullOrWhiteSpace(gender)
            ? null
            : Enum.TryParse<GenderType>(gender.Trim(), true, out var parsedGender)
                ? parsedGender
                : null;
}
