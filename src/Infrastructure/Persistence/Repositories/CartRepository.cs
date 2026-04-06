using elearn_server.Infrastructure.Persistence.Repositories.IRepository;
using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class CartRepository(AppDbContext context) : ICartRepository
{
    public Task<User?> GetUserAsync(int userId) => context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
    public Task<Course?> GetCourseAsync(int courseId) => context.Courses.SingleOrDefaultAsync(c => c.CourseId == courseId);
    public Task<Cart?> GetByUserIdAsync(int userId) => context.Carts.Include(c => c.CartItems).ThenInclude(i => i.Course).SingleOrDefaultAsync(c => c.UserId == userId);
    public Task AddAsync(Cart cart) => context.Carts.AddAsync(cart).AsTask();
    public Task<List<User>> GetUsersWithoutCartsAsync()
    {
        var userIdsWithCart = context.Carts.Select(c => c.UserId);
        return context.Users.Where(u => !userIdsWithCart.Contains(u.UserId)).ToListAsync();
    }
    public Task SaveChangesAsync() => context.SaveChangesAsync();
    public void RemoveCartItems(IEnumerable<CartItem> items) => context.CartItems.RemoveRange(items);
    public Task AddCartItemAsync(CartItem item) => context.CartItems.AddAsync(item).AsTask();
}

