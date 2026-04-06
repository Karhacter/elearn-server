
using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class WishlistRepository(AppDbContext context) : IWishlistRepository
{
    public Task<List<Wishlist>> GetByUserIdAsync(int userId) => context.Wishlists.Include(w => w.Course).AsNoTracking().Where(w => w.UserId == userId).ToListAsync();
    public Task<Wishlist?> GetByIdAsync(int id) => context.Wishlists.Include(w => w.Course).SingleOrDefaultAsync(w => w.Id == id);
    public Task<Wishlist?> GetByUserAndCourseAsync(int userId, int courseId) => context.Wishlists.SingleOrDefaultAsync(w => w.UserId == userId && w.CourseId == courseId);
    public Task<bool> UserExistsAsync(int userId) => context.Users.AnyAsync(u => u.UserId == userId);
    public Task<Course?> GetCourseAsync(int courseId) => context.Courses.SingleOrDefaultAsync(c => c.CourseId == courseId);
    public Task AddAsync(Wishlist wishlist) => context.Wishlists.AddAsync(wishlist).AsTask();
    public void Remove(Wishlist wishlist) => context.Wishlists.Remove(wishlist);
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}

