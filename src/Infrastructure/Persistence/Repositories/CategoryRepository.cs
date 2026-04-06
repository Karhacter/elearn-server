using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public Task<List<Category>> GetAllAsync() => context.Categories.AsNoTracking().ToListAsync();
    public Task<Category?> GetByIdAsync(int id) => context.Categories.SingleOrDefaultAsync(c => c.Id == id);
    public Task<Category?> GetByNameAsync(string name) => context.Categories.SingleOrDefaultAsync(c => c.Name != null && c.Name.ToLower() == name.ToLower());
    public Task AddAsync(Category category) => context.Categories.AddAsync(category).AsTask();
    public void Remove(Category category) => context.Categories.Remove(category);
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}

