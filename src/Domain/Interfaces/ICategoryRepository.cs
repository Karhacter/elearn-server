using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category?> GetByNameAsync(string name);
    Task AddAsync(Category category);
    void Remove(Category category);
    Task SaveChangesAsync();
}

