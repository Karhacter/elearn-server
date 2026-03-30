using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using elearn_server.Application.Common;
using elearn_server.Application.Responses;
using elearn_server.Application.DTOs;
using elearn_server.Application.Mappings;
using elearn_server.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using elearn_server.Application.Interfaces;
using elearn_server.Domain.Interfaces;
using elearn_server.Common.Options;

namespace elearn_server.Infrastructure.Services.Core.Auth;


public class AuthService(
    IAuthRepository authRepository,
    IEmailService emailService,
    IConfiguration configuration,
    IWebHostEnvironment environment,
    IOptions<JwtOptions> jwtOptions,
    IOptions<AuthSecurityOptions> authSecurityOptions) : IAuthService
{
    private readonly PasswordHasher<User> _passwordHasher = new();
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly AuthSecurityOptions _authSecurityOptions = authSecurityOptions.Value;

    public async Task<ServiceResult<IReadOnlyCollection<AuthenticatedUserResponse>>> GetUsersAsync() =>
        ServiceResult<IReadOnlyCollection<AuthenticatedUserResponse>>.Ok((await authRepository.GetUsersAsync()).Select(u => u.ToAuthenticatedUserResponse()).ToList());

    public async Task<ServiceResult<AuthenticatedUserResponse>> GetUserByIdAsync(int id)
    {
        var user = await authRepository.GetUserByIdAsync(id);
        return user is null
            ? ServiceResult<AuthenticatedUserResponse>.Fail(StatusCodes.Status404NotFound, "User not found.")
            : ServiceResult<AuthenticatedUserResponse>.Ok(user.ToAuthenticatedUserResponse());
    }

    public async Task<ServiceResult<LoginResponse>> LoginAsync(UserLoginDTO request, string? ipAddress, string? userAgent)
    {
        var user = await authRepository.GetUserByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (user is null)
        {
            await WriteAuditLogAsync(null, "login", "failed", ipAddress, userAgent, "Email not found.");
            return ServiceResult<LoginResponse>.Fail(StatusCodes.Status401Unauthorized, "Email or password is incorrect.");
        }

        if (_passwordHasher.VerifyHashedPassword(user, user.Password, request.Password.Trim()) == PasswordVerificationResult.Failed)
        {
            await WriteAuditLogAsync(user.UserId, "login", "failed", ipAddress, userAgent, "Password mismatch.");
            return ServiceResult<LoginResponse>.Fail(StatusCodes.Status401Unauthorized, "Email or password is incorrect.");
        }

        if (_authSecurityOptions.RequireEmailVerification && !user.IsEmailVerified)
        {
            await WriteAuditLogAsync(user.UserId, "login", "blocked", ipAddress, userAgent, "Email is not verified.");
            return ServiceResult<LoginResponse>.Fail(StatusCodes.Status403Forbidden, "Email is not verified.");
        }

        var accessToken = CreateAccessToken(user);
        var refreshToken = await CreateRefreshTokenAsync(user.UserId, ipAddress);
        await WriteAuditLogAsync(user.UserId, "login", "success", ipAddress, userAgent, null);

        return ServiceResult<LoginResponse>.Ok(
            new LoginResponse
            {
                User = user.ToAuthenticatedUserResponse(),
                ExpiresAtUtc = accessToken.ExpiresAtUtc,
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken.RawToken
            },
            "Login successful.");
    }

    public async Task<ServiceResult<AuthenticatedUserResponse>> RegisterAsync(UserCreateDTO request, string verificationBaseUrl)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await authRepository.GetUserByEmailAsync(normalizedEmail) is not null)
        {
            return ServiceResult<AuthenticatedUserResponse>.Fail(StatusCodes.Status409Conflict, "Email is already registered.");
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            PhoneNumber = request.PhoneNumber.Trim(),
            Role = request.Role.Trim(),
            ProfilePicture = string.IsNullOrWhiteSpace(request.ProfilePicture) ? null : request.ProfilePicture.Trim(),
            IsEmailVerified = !_authSecurityOptions.RequireEmailVerification,
            EmailVerifiedAt = _authSecurityOptions.RequireEmailVerification ? null : DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "system"
        };

        user.Password = _passwordHasher.HashPassword(user, request.Password.Trim());
        await authRepository.AddUserAsync(user);
        await authRepository.SaveChangesAsync();

        if (_authSecurityOptions.RequireEmailVerification)
        {
            await IssueEmailVerificationAsync(user, verificationBaseUrl);
        }

        return ServiceResult<AuthenticatedUserResponse>.Created(user.ToAuthenticatedUserResponse(), "User registered successfully.");
    }

    public async Task<ServiceResult<LoginResponse>> RefreshTokenAsync(RefreshTokenRequestDTO request, string? ipAddress, string? userAgent)
    {
        var refreshToken = await authRepository.GetRefreshTokenAsync(HashToken(request.RefreshToken));
        if (refreshToken is null || refreshToken.IsRevoked || refreshToken.IsUsed || refreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            await WriteAuditLogAsync(null, "refresh-token", "failed", ipAddress, userAgent, "Invalid refresh token.");
            return ServiceResult<LoginResponse>.Fail(StatusCodes.Status401Unauthorized, "Refresh token is invalid or expired.");
        }

        var user = refreshToken.User;
        if (_authSecurityOptions.RequireEmailVerification && !user.IsEmailVerified)
        {
            return ServiceResult<LoginResponse>.Fail(StatusCodes.Status403Forbidden, "Email is not verified.");
        }

        refreshToken.IsUsed = true;
        refreshToken.UsedAt = DateTime.UtcNow;

        var accessToken = CreateAccessToken(user);
        var nextRefreshToken = await CreateRefreshTokenAsync(user.UserId, ipAddress);
        await WriteAuditLogAsync(user.UserId, "refresh-token", "success", ipAddress, userAgent, null);

        return ServiceResult<LoginResponse>.Ok(new LoginResponse
        {
            User = user.ToAuthenticatedUserResponse(),
            ExpiresAtUtc = accessToken.ExpiresAtUtc,
            AccessToken = accessToken.Token,
            RefreshToken = nextRefreshToken.RawToken
        }, "Token refreshed successfully.");
    }

    public async Task<ServiceResult<object>> VerifyEmailAsync(VerifyEmailRequestDTO request)
    {
        var verificationToken = await authRepository.GetLatestEmailVerificationTokenAsync(HashToken(request.Token));
        if (verificationToken is null || verificationToken.IsUsed || verificationToken.ExpiresAt <= DateTime.UtcNow)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status400BadRequest, "Email verification token is invalid or has expired.");
        }

        verificationToken.IsUsed = true;
        verificationToken.UsedAt = DateTime.UtcNow;
        verificationToken.User.IsEmailVerified = true;
        verificationToken.User.EmailVerifiedAt = DateTime.UtcNow;

        await authRepository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Email verified successfully.");
    }

    public async Task<ServiceResult<object>> ForgotPasswordAsync(ForgotPasswordRequestDTO request)
    {
        var user = await authRepository.GetUserByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (user is null)
        {
            return ServiceResult<object>.Ok(null, "If an account with that email exists, a password reset link has been sent.");
        }

        var activeTokens = await authRepository.GetActivePasswordResetTokensAsync(user.UserId);
        foreach (var token in activeTokens)
        {
            token.IsUsed = true;
            token.UsedAt = DateTime.UtcNow;
        }

        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var passwordResetToken = new PasswordResetToken
        {
            UserId = user.UserId,
            TokenHash = HashToken(rawToken),
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };

        await authRepository.AddPasswordResetTokenAsync(passwordResetToken);
        await authRepository.SaveChangesAsync();

        var frontendBaseUrl = configuration["App:FrontendBaseUrl"] ?? "http://localhost:3000";
        var resetLink = $"{frontendBaseUrl.TrimEnd('/')}/reset-password?token={Uri.EscapeDataString(rawToken)}";

        if (emailService.IsConfigured())
        {
            await emailService.SendPasswordResetEmailAsync(user.Email, user.FullName, resetLink);
        }

        return environment.IsDevelopment()
            ? ServiceResult<object>.Ok(new { resetLink }, "Password reset request processed successfully.")
            : ServiceResult<object>.Ok(null, "If an account with that email exists, a password reset link has been sent.");
    }

    public async Task<ServiceResult<object>> ResetPasswordAsync(ResetPasswordRequestDTO request)
    {
        var resetToken = await authRepository.GetLatestPasswordResetTokenAsync(HashToken(request.Token));
        if (resetToken is null || resetToken.IsUsed || resetToken.ExpiresAt <= DateTime.UtcNow)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status400BadRequest, "Reset token is invalid or has expired.");
        }

        var user = await authRepository.GetUserByIdAsync(resetToken.UserId);
        if (user is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }

        user.Password = _passwordHasher.HashPassword(user, request.NewPassword.Trim());
        resetToken.IsUsed = true;
        resetToken.UsedAt = DateTime.UtcNow;
        await authRepository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Password has been reset successfully.");
    }

    public async Task<ServiceResult<object>> ChangePasswordAsync(int userId, ChangePasswordRequestDTO request, string? ipAddress, string? userAgent)
    {
        var user = await authRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }

        if (_passwordHasher.VerifyHashedPassword(user, user.Password, request.CurrentPassword.Trim()) == PasswordVerificationResult.Failed)
        {
            await WriteAuditLogAsync(user.UserId, "change-password", "failed", ipAddress, userAgent, "Current password mismatch.");
            return ServiceResult<object>.Fail(StatusCodes.Status400BadRequest, "Current password is incorrect.");
        }

        if (_passwordHasher.VerifyHashedPassword(user, user.Password, request.NewPassword.Trim()) != PasswordVerificationResult.Failed)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status400BadRequest, "New password must be different from the current password.");
        }

        user.Password = _passwordHasher.HashPassword(user, request.NewPassword.Trim());
        user.UpdatedAt = DateTime.UtcNow;
        await authRepository.SaveChangesAsync();
        await WriteAuditLogAsync(user.UserId, "change-password", "success", ipAddress, userAgent, null);
        return ServiceResult<object>.Ok(null, "Password changed successfully.");
    }

    public async Task<ServiceResult<AuthCheckResponse>> CheckAuthAsync(string token, ClaimsPrincipal user)
    {
        if (!string.IsNullOrWhiteSpace(token) && await authRepository.GetBlacklistedTokenAsync(token) is not null)
        {
            return ServiceResult<AuthCheckResponse>.Fail(StatusCodes.Status401Unauthorized, "Token is blacklisted.");
        }

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return ServiceResult<AuthCheckResponse>.Fail(StatusCodes.Status401Unauthorized, "Invalid user session.");
        }

        return ServiceResult<AuthCheckResponse>.Ok(new AuthCheckResponse
        {
            UserId = userId,
            Name = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
            Role = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty
        });
    }

    public async Task<ServiceResult<object>> LogoutAsync(string token, string? ipAddress, string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return ServiceResult<object>.Fail(StatusCodes.Status400BadRequest, "No token provided.");
        }

        try
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            await authRepository.AddBlacklistedTokenAsync(new BlacklistedToken
            {
                Token = token,
                Expiration = jwtToken.ValidTo
            });
            await authRepository.SaveChangesAsync();
            await WriteAuditLogAsync(
                int.TryParse(jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out var userId) ? userId : null,
                "logout",
                "success",
                ipAddress,
                userAgent,
                null);
            return ServiceResult<object>.Ok(null, "Logged out successfully.");
        }
        catch
        {
            return ServiceResult<object>.Fail(StatusCodes.Status400BadRequest, "Invalid token.");
        }
    }

    private AccessTokenResult CreateAccessToken(User user)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role)
            ]),
            Expires = DateTime.UtcNow.AddDays(_jwtOptions.ExpireDays),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return new AccessTokenResult(handler.WriteToken(token), token.ValidTo);
    }

    private async Task<RefreshTokenIssueResult> CreateRefreshTokenAsync(int userId, string? ipAddress)
    {
        var activeTokens = await authRepository.GetActiveRefreshTokensAsync(userId);
        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }

        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        await authRepository.AddRefreshTokenAsync(new RefreshToken
        {
            UserId = userId,
            TokenHash = HashToken(rawToken),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpireDays),
            CreatedByIp = ipAddress
        });

        await authRepository.SaveChangesAsync();
        return new RefreshTokenIssueResult(rawToken);
    }

    private async Task IssueEmailVerificationAsync(User user, string verificationBaseUrl)
    {
        var activeTokens = await authRepository.GetActiveEmailVerificationTokensAsync(user.UserId);
        foreach (var token in activeTokens)
        {
            token.IsUsed = true;
            token.UsedAt = DateTime.UtcNow;
        }

        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        await authRepository.AddEmailVerificationTokenAsync(new EmailVerificationToken
        {
            UserId = user.UserId,
            TokenHash = HashToken(rawToken),
            ExpiresAt = DateTime.UtcNow.AddHours(_authSecurityOptions.EmailVerificationExpireHours)
        });
        await authRepository.SaveChangesAsync();

        var frontendBaseUrl = string.IsNullOrWhiteSpace(verificationBaseUrl)
            ? (configuration["App:FrontendBaseUrl"] ?? "http://localhost:4200")
            : verificationBaseUrl;
        var verificationLink = $"{frontendBaseUrl.TrimEnd('/')}/verify-email?token={Uri.EscapeDataString(rawToken)}";

        if (emailService.IsConfigured())
        {
            await emailService.SendEmailVerificationAsync(user.Email, user.FullName, verificationLink);
        }
    }

    private async Task WriteAuditLogAsync(int? userId, string action, string result, string? ipAddress, string? userAgent, string? detail)
    {
        await authRepository.AddAuditLogAsync(new AuditLog
        {
            UserId = userId,
            Action = action,
            Result = result,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Detail = detail,
            CreatedAt = DateTime.UtcNow
        });
        await authRepository.SaveChangesAsync();
    }

    private static string HashToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes);
    }

    private sealed record AccessTokenResult(string Token, DateTime ExpiresAtUtc);
    private sealed record RefreshTokenIssueResult(string RawToken);
}
