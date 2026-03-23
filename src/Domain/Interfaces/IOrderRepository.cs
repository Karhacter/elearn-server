using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<List<Order>> GetByUserIdAsync(int userId);
    Task<StatusOrder?> GetStatusByIdAsync(int statusId);
    Task<StatusOrder?> GetStatusByNameAsync(string name);
    Task<Course?> GetCourseByIdAsync(int courseId);
    Task<bool> UserExistsAsync(int userId);
    Task AddAsync(Order order);
    void Remove(Order order);
    Task SaveChangesAsync();
}

