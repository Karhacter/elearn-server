using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    void Remove(User user);
    Task SaveChangesAsync();
}

