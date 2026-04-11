using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface INotificationService
{
    /// <summary>Creates a notification for a specific user (Admin/System use).</summary>
    Task<ServiceResult<NotificationResponse>> CreateAsync(CreateNotificationRequest request);

    /// <summary>Returns all non-deleted notifications for the given user, newest first.</summary>
    Task<ServiceResult<IReadOnlyCollection<NotificationResponse>>> GetByUserAsync(int userId);

    /// <summary>Returns a quick unread/total summary for badge display.</summary>
    Task<ServiceResult<NotificationSummaryResponse>> GetSummaryAsync(int userId);

    /// <summary>Marks a single notification as read (must belong to the requesting user).</summary>
    Task<ServiceResult<NotificationResponse>> MarkAsReadAsync(int notificationId, int userId);

    /// <summary>Marks ALL unread notifications for the user as read.</summary>
    Task<ServiceResult<object>> MarkAllAsReadAsync(int userId);

    /// <summary>Soft-deletes a notification (must belong to the requesting user).</summary>
    Task<ServiceResult<object>> DeleteAsync(int notificationId, int userId);
}
