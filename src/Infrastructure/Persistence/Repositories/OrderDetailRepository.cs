using elearn_server.Infrastructure.Persistence;
using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using elearn_server.Domain.Interfaces;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class OrderDetailRepository(AppDbContext context) : IOrderDetailRepository
{
    private IQueryable<OrderDetail> BaseQuery() => context.OrderDetails.Include(d => d.Order).Include(d => d.Course);
    public Task<List<OrderDetail>> GetAllAsync() => BaseQuery().AsNoTracking().ToListAsync();
    public Task<OrderDetail?> GetByIdAsync(int id) => BaseQuery().SingleOrDefaultAsync(d => d.Id == id);
    public Task<List<OrderDetail>> GetByOrderIdAsync(int orderId) => BaseQuery().AsNoTracking().Where(d => d.OrderId == orderId).ToListAsync();
    public Task<Order?> GetOrderAsync(int orderId) => context.Orders.SingleOrDefaultAsync(o => o.OrderID == orderId);
    public Task<Course?> GetCourseAsync(int courseId) => context.Courses.SingleOrDefaultAsync(c => c.CourseId == courseId);
    public Task AddAsync(OrderDetail detail) => context.OrderDetails.AddAsync(detail).AsTask();
    public void Remove(OrderDetail detail) => context.OrderDetails.Remove(detail);
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}

