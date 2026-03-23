using elearn_server.Infrastructure.Persistence;
using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using elearn_server.Domain.Interfaces;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class AuthRepository(AppDbContext context) : IAuthRepository
{
    public Task<List<User>> GetUsersAsync() => context.Users.AsNoTracking().ToListAsync();
    public Task<User?> GetUserByIdAsync(int userId) => context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
    public Task<User?> GetUserByEmailAsync(string email) => context.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    public Task AddUserAsync(User user) => context.Users.AddAsync(user).AsTask();
    public Task<List<PasswordResetToken>> GetActivePasswordResetTokensAsync(int userId) =>
        context.PasswordResetTokens.Where(t => t.UserId == userId && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow).ToListAsync();
    public Task AddPasswordResetTokenAsync(PasswordResetToken token) => context.PasswordResetTokens.AddAsync(token).AsTask();
    public Task<PasswordResetToken?> GetLatestPasswordResetTokenAsync(string tokenHash) =>
        context.PasswordResetTokens.OrderByDescending(t => t.Id).FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
    public Task<BlacklistedToken?> GetBlacklistedTokenAsync(string token) => context.BlacklistedTokens.SingleOrDefaultAsync(t => t.Token == token);
    public Task AddBlacklistedTokenAsync(BlacklistedToken token) => context.BlacklistedTokens.AddAsync(token).AsTask();
    public Task AddRefreshTokenAsync(RefreshToken token) => context.RefreshTokens.AddAsync(token).AsTask();
    public Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash) =>
        context.RefreshTokens.Include(t => t.User).OrderByDescending(t => t.Id).FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
    public Task<List<RefreshToken>> GetActiveRefreshTokensAsync(int userId) =>
        context.RefreshTokens.Where(t => t.UserId == userId && !t.IsRevoked && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow).ToListAsync();
    public Task AddEmailVerificationTokenAsync(EmailVerificationToken token) => context.EmailVerificationTokens.AddAsync(token).AsTask();
    public Task<EmailVerificationToken?> GetLatestEmailVerificationTokenAsync(string tokenHash) =>
        context.EmailVerificationTokens.Include(t => t.User).OrderByDescending(t => t.Id).FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
    public Task<List<EmailVerificationToken>> GetActiveEmailVerificationTokensAsync(int userId) =>
        context.EmailVerificationTokens.Where(t => t.UserId == userId && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow).ToListAsync();
    public Task AddAuditLogAsync(AuditLog auditLog) => context.AuditLogs.AddAsync(auditLog).AsTask();
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}

