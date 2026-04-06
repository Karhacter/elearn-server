using System.Security.Claims;
using elearn_server.Application.Common;
using elearn_server.Application.DTOs;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<IReadOnlyCollection<AuthenticatedUserResponse>>> GetUsersAsync();
    Task<ServiceResult<AuthenticatedUserResponse>> GetUserByIdAsync(int id);
    Task<ServiceResult<LoginResponse>> LoginAsync(UserLoginDTO request, string? ipAddress, string? userAgent);
    Task<ServiceResult<AuthenticatedUserResponse>> RegisterAsync(RegisterUserDTO request, string verificationBaseUrl);
    Task<ServiceResult<LoginResponse>> RefreshTokenAsync(RefreshTokenRequestDTO request, string? ipAddress, string? userAgent);
    Task<ServiceResult<object>> VerifyEmailAsync(VerifyEmailRequestDTO request);
    Task<ServiceResult<object>> ForgotPasswordAsync(ForgotPasswordRequestDTO request);
    Task<ServiceResult<object>> ResetPasswordAsync(ResetPasswordRequestDTO request);
    Task<ServiceResult<object>> ChangePasswordAsync(int userId, ChangePasswordRequestDTO request, string? ipAddress, string? userAgent);
    Task<ServiceResult<AuthCheckResponse>> CheckAuthAsync(string token, ClaimsPrincipal user);
    Task<ServiceResult<object>> LogoutAsync(string token, string? ipAddress, string? userAgent);
}
