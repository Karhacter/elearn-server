using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class NotificationRepository(AppDbContext context) : INotificationRepository
{
    public Task<List<Notification>> GetByUserIdAsync(int userId) =>
        context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public Task<Notification?> GetByIdAsync(int id) =>
        context.Notifications
            .SingleOrDefaultAsync(n => n.Id == id && !n.IsDeleted);

    public Task<int> CountUnreadByUserIdAsync(int userId) =>
        context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted);

    public Task<int> CountByUserIdAsync(int userId) =>
        context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsDeleted);

    public Task<List<Notification>> GetUnreadByUserIdAsync(int userId) =>
        context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
            .ToListAsync();

    public Task AddAsync(Notification notification) =>
        context.Notifications.AddAsync(notification).AsTask();

    public void Update(Notification notification) =>
        context.Notifications.Update(notification);

    public Task SaveChangesAsync() =>
        context.SaveChangesAsync();
}
