using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public interface IPaymentRepository
{
    Task<List<Payment>> GetByUserIdAsync(int userId);
    Task<User?> GetUserAsync(int userId);
    Task<Course?> GetCourseAsync(int courseId);
    Task<Order?> GetOrderAsync(int orderId);
    Task<StatusOrder?> GetStatusByNameAsync(string name);
    Task AddAsync(Payment payment);
    Task SaveChangesAsync();
}

