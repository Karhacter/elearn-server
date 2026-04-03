using elearn_server.Application.Common;
using elearn_server.Application.DTOs;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Mappings;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;

namespace elearn_server.Infrastructure.Services.Core.Users;

public class UserService(IUserRepository repository, IFileStorageService fileStorageService) : IUserService
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public async Task<ServiceResult<IReadOnlyCollection<AuthenticatedUserResponse>>> GetAllAsync() =>
        ServiceResult<IReadOnlyCollection<AuthenticatedUserResponse>>.Ok((await repository.GetAllAsync()).Select(u => u.ToAuthenticatedUserResponse()).ToList());

    public async Task<ServiceResult<AuthenticatedUserResponse>> GetByIdAsync(int id)
    {
        var user = await repository.GetByIdAsync(id);
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
        user.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync();
        return ServiceResult<AuthenticatedUserResponse>.Ok(user.ToAuthenticatedUserResponse(), "User updated successfully.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }

        repository.Remove(user);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "User deleted successfully.");
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
}
