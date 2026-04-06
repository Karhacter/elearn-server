
using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    private IQueryable<Order> BaseQuery() => context.Orders.Include(o => o.OrderDetails).ThenInclude(d => d.Course);
    public Task<List<Order>> GetAllAsync() => BaseQuery().AsNoTracking().ToListAsync();
    public Task<Order?> GetByIdAsync(int id) => BaseQuery().SingleOrDefaultAsync(o => o.OrderID == id);
    public Task<List<Order>> GetByUserIdAsync(int userId) => BaseQuery().AsNoTracking().Where(o => o.user_id == userId).ToListAsync();
    public Task<Course?> GetCourseByIdAsync(int courseId) => context.Courses.SingleOrDefaultAsync(c => c.CourseId == courseId);
    public Task<bool> UserExistsAsync(int userId) => context.Users.AnyAsync(u => u.UserId == userId);
    public Task AddAsync(Order order) => context.Orders.AddAsync(order).AsTask();
    public void Remove(Order order) => context.Orders.Remove(order);
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}

