using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence.Repositories.IRepository;

public interface INotificationRepository
{
    Task<List<Notification>> GetByUserIdAsync(int userId);
    Task<Notification?> GetByIdAsync(int id);
    Task<int> CountUnreadByUserIdAsync(int userId);
    Task<int> CountByUserIdAsync(int userId);
    Task<List<Notification>> GetUnreadByUserIdAsync(int userId);
    Task AddAsync(Notification notification);
    void Update(Notification notification);
    Task SaveChangesAsync();
}
