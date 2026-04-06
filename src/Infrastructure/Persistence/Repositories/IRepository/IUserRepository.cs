using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories.IRepository;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<List<User>> GetPagedAsync(int page, int pageSize);
    Task<int> CountAsync();
    Task<List<User>> GetAllDeletedAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByIdIncludingDeletedAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    void Remove(User user);
    void RemovePermanently(User user);
    Task SaveChangesAsync();
}
