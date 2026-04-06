using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories.IRepository;

public interface IPaymentRepository
{
    Task<List<Payment>> GetByUserIdAsync(int userId);
    Task<User?> GetUserAsync(int userId);
    Task<Course?> GetCourseAsync(int courseId);
    Task<Order?> GetOrderAsync(int orderId);

    Task AddAsync(Payment payment);
    Task SaveChangesAsync();
}


