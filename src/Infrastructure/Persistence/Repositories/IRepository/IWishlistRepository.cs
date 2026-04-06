using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories.IRepository;

public interface IWishlistRepository
{
    Task<List<Wishlist>> GetByUserIdAsync(int userId);
    Task<Wishlist?> GetByIdAsync(int id);
    Task<Wishlist?> GetByUserAndCourseAsync(int userId, int courseId);
    Task<bool> UserExistsAsync(int userId);
    Task<Course?> GetCourseAsync(int courseId);
    Task AddAsync(Wishlist wishlist);
    void Remove(Wishlist wishlist);
    Task SaveChangesAsync();
}

