using elearn_server.Infrastructure.Persistence;
using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using elearn_server.Domain.Interfaces;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<List<User>> GetAllAsync() => context.Users.AsNoTracking().ToListAsync();
    public Task<User?> GetByIdAsync(int id) => context.Users.SingleOrDefaultAsync(u => u.UserId == id);
    public Task<User?> GetByEmailAsync(string email) => context.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    public Task AddAsync(User user) => context.Users.AddAsync(user).AsTask();
    public void Remove(User user) => context.Users.Remove(user);
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}

