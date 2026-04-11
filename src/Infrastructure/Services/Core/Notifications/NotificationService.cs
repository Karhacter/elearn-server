using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Mappings;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;
using Microsoft.AspNetCore.Http;

namespace elearn_server.Infrastructure.Services.Core.Notifications;

public class NotificationService(INotificationRepository notificationRepository) : INotificationService
{
    public async Task<ServiceResult<NotificationResponse>> CreateAsync(CreateNotificationRequest request)
    {
        var notification = new Notification
        {
            UserId    = request.UserId,
            Title     = request.Title,
            Message   = request.Message,
            Type      = request.Type,
            ActionUrl = request.ActionUrl,
            IsRead    = false,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "system"
        };

        await notificationRepository.AddAsync(notification);
        await notificationRepository.SaveChangesAsync();

        return ServiceResult<NotificationResponse>.Created(
            notification.ToResponse(),
            "Notification created successfully.");
    }

    public async Task<ServiceResult<IReadOnlyCollection<NotificationResponse>>> GetByUserAsync(int userId)
    {
        var notifications = await notificationRepository.GetByUserIdAsync(userId);
        var responses = notifications
            .Select(n => n.ToResponse())
            .ToList()
            .AsReadOnly();

        return ServiceResult<IReadOnlyCollection<NotificationResponse>>.Ok(responses);
    }

    public async Task<ServiceResult<NotificationSummaryResponse>> GetSummaryAsync(int userId)
    {
        var total  = await notificationRepository.CountByUserIdAsync(userId);
        var unread = await notificationRepository.CountUnreadByUserIdAsync(userId);

        return ServiceResult<NotificationSummaryResponse>.Ok(new NotificationSummaryResponse
        {
            TotalCount  = total,
            UnreadCount = unread
        });
    }

    public async Task<ServiceResult<NotificationResponse>> MarkAsReadAsync(int notificationId, int userId)
    {
        var notification = await notificationRepository.GetByIdAsync(notificationId);

        if (notification is null)
            return ServiceResult<NotificationResponse>.Fail(
                StatusCodes.Status404NotFound,
                "Notification not found.");

        if (notification.UserId != userId)
            return ServiceResult<NotificationResponse>.Fail(
                StatusCodes.Status403Forbidden,
                "You are not authorised to modify this notification.");

        if (notification.IsRead)
            return ServiceResult<NotificationResponse>.Ok(
                notification.ToResponse(),
                "Notification was already marked as read.");

        notification.IsRead    = true;
        notification.UpdatedAt = DateTime.UtcNow;
        notification.UpdatedBy = userId.ToString();

        notificationRepository.Update(notification);
        await notificationRepository.SaveChangesAsync();

        return ServiceResult<NotificationResponse>.Ok(
            notification.ToResponse(),
            "Notification marked as read.");
    }

    public async Task<ServiceResult<object>> MarkAllAsReadAsync(int userId)
    {
        var unread = await notificationRepository.GetUnreadByUserIdAsync(userId);

        if (unread.Count == 0)
            return ServiceResult<object>.Ok(null, "No unread notifications.");

        foreach (var n in unread)
        {
            n.IsRead    = true;
            n.UpdatedAt = DateTime.UtcNow;
            n.UpdatedBy = userId.ToString();
            notificationRepository.Update(n);
        }

        await notificationRepository.SaveChangesAsync();

        return ServiceResult<object>.Ok(
            null,
            $"{unread.Count} notification(s) marked as read.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int notificationId, int userId)
    {
        var notification = await notificationRepository.GetByIdAsync(notificationId);

        if (notification is null)
            return ServiceResult<object>.Fail(
                StatusCodes.Status404NotFound,
                "Notification not found.");

        if (notification.UserId != userId)
            return ServiceResult<object>.Fail(
                StatusCodes.Status403Forbidden,
                "You are not authorised to delete this notification.");

        // Soft delete — consistent with BaseEntity pattern (DeletedAt / DeletedBy)
        notification.IsDeleted = true;
        notification.DeletedAt = DateTime.UtcNow;
        notification.DeletedBy = userId.ToString();

        notificationRepository.Update(notification);
        await notificationRepository.SaveChangesAsync();

        return ServiceResult<object>.Ok(null, "Notification deleted successfully.");
    }
}
