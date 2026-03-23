using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public interface IOrderDetailRepository
{
    Task<List<OrderDetail>> GetAllAsync();
    Task<OrderDetail?> GetByIdAsync(int id);
    Task<List<OrderDetail>> GetByOrderIdAsync(int orderId);
    Task<Order?> GetOrderAsync(int orderId);
    Task<Course?> GetCourseAsync(int courseId);
    Task AddAsync(OrderDetail detail);
    void Remove(OrderDetail detail);
    Task SaveChangesAsync();
}

