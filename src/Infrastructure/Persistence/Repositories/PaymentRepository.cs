using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class PaymentRepository(AppDbContext context) : IPaymentRepository
{
    public Task<List<Payment>> GetByUserIdAsync(int userId) => context.Payments.Include(p => p.Course).AsNoTracking().Where(p => p.UserId == userId).ToListAsync();
    public Task<User?> GetUserAsync(int userId) => context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
    public Task<Course?> GetCourseAsync(int courseId) => context.Courses.SingleOrDefaultAsync(c => c.CourseId == courseId);
    public Task<Order?> GetOrderAsync(int orderId) => context.Orders.SingleOrDefaultAsync(o => o.OrderID == orderId);
    public Task AddAsync(Payment payment) => context.Payments.AddAsync(payment).AsTask();
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}



