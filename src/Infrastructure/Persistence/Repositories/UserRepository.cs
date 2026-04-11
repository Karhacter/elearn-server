using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<List<User>> GetAllAsync() => context.Users.AsNoTracking().ToListAsync();
    public Task<List<User>> GetPagedAsync(int page, int pageSize) =>
        context.Users.Where(u => u.Role == "Student").AsNoTracking().Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    public Task<int> CountAsync() => context.Users.Where(u => u.Role == "Student").CountAsync();

    public Task<List<User>> GetPagedInstructorsAsync(int page, int pageSize) =>
        context.Users.Where(u => u.Role == "Instructor").AsNoTracking().Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    public Task<int> CountInstructorsAsync() => context.Users.Where(u => u.Role == "Instructor").CountAsync();

    public Task<List<User>> GetAllDeletedAsync() => context.Users.IgnoreQueryFilters().Where(u => u.IsDeleted && u.Role == "Student").AsNoTracking().ToListAsync();

    public Task<List<User>> GetAllDeletedInstructorsAsync() => context.Users.IgnoreQueryFilters().Where(u => u.IsDeleted && u.Role == "Instructor").AsNoTracking().ToListAsync();

    public Task<User?> GetByIdAsync(int id) => context.Users.SingleOrDefaultAsync(u => u.UserId == id);

    public Task<User?> GetByIdIncludingDeletedAsync(int id) => context.Users.IgnoreQueryFilters().SingleOrDefaultAsync(u => u.UserId == id);
    public Task<User?> GetByEmailAsync(string email) => context.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    public Task AddAsync(User user) => context.Users.AddAsync(user).AsTask();
    public void Remove(User user)
    {
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.DeletedBy = "Admin";
        user.UpdatedAt = DateTime.UtcNow;
    }

    public void RemovePermanently(User user) => context.Users.Remove(user);

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
