using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<List<User>> GetAllAsync() => context.Users.AsNoTracking().ToListAsync();
    public Task<User?> GetByIdAsync(int id) => context.Users.SingleOrDefaultAsync(u => u.UserId == id);
    public Task<User?> GetByEmailAsync(string email) => context.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    public Task AddAsync(User user) => context.Users.AddAsync(user).AsTask();
    public void Remove(User user)
    {
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.DeletedBy = "Admin";
        user.UpdatedAt = DateTime.UtcNow;
    }
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}

