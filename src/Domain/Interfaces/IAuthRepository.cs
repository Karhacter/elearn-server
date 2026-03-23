using elearn_server.Domain.Entities;

namespace elearn_server.Domain.Interfaces;

public interface IAuthRepository
{
    Task<List<User>> GetUsersAsync();
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task<List<PasswordResetToken>> GetActivePasswordResetTokensAsync(int userId);
    Task AddPasswordResetTokenAsync(PasswordResetToken token);
    Task<PasswordResetToken?> GetLatestPasswordResetTokenAsync(string tokenHash);
    Task<BlacklistedToken?> GetBlacklistedTokenAsync(string token);
    Task AddBlacklistedTokenAsync(BlacklistedToken token);
    Task AddRefreshTokenAsync(RefreshToken token);
    Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash);
    Task<List<RefreshToken>> GetActiveRefreshTokensAsync(int userId);
    Task AddEmailVerificationTokenAsync(EmailVerificationToken token);
    Task<EmailVerificationToken?> GetLatestEmailVerificationTokenAsync(string tokenHash);
    Task<List<EmailVerificationToken>> GetActiveEmailVerificationTokensAsync(int userId);
    Task AddAuditLogAsync(AuditLog auditLog);
    Task SaveChangesAsync();
}

