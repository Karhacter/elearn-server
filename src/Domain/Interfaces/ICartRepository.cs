using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public interface ICartRepository
{
    Task<User?> GetUserAsync(int userId);
    Task<Course?> GetCourseAsync(int courseId);
    Task<Cart?> GetByUserIdAsync(int userId);
    Task AddAsync(Cart cart);
    Task<List<User>> GetUsersWithoutCartsAsync();
    Task SaveChangesAsync();
    void RemoveCartItems(IEnumerable<CartItem> items);
    Task AddCartItemAsync(CartItem item);
}

