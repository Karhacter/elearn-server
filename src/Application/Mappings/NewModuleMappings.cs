using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;

namespace elearn_server.Application.Mappings;

public static class NewModuleMappings
{
    public static NotificationResponse ToResponse(this Notification notification) => new()
    {
        Id = notification.Id,
        UserId = notification.UserId,
        Title = notification.Title,
        Message = notification.Message,
        Type = notification.Type.ToString(),
        IsRead = notification.IsRead,
        ActionUrl = notification.ActionUrl,
        CreatedAt = notification.CreatedAt
    };

    public static TopicResponse ToResponse(this Topic topic) => new()
    {
        Id = topic.Id,
        Name = topic.Name,
        Description = topic.Description,
        IsDeleted = topic.IsDeleted
    };

    public static PostResponse ToResponse(this Post post) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Content = post.Content,
        ThumbnailUrl = post.ThumbnailUrl,
        TopicId = post.TopicId,
        TopicName = post.Topic?.Name,
        IsDeleted = post.IsDeleted
    };

    public static ContactResponse ToResponse(this Contact contact) => new()
    {
        Id = contact.Id,
        FullName = contact.FullName,
        Email = contact.Email,
        Message = contact.Message,
        Status = contact.Status,
        IsDeleted = contact.IsDeleted
    };

    public static MenuResponse ToResponse(this Menu menu) => new()
    {
        Id = menu.Id,
        Name = menu.Name,
        Url = menu.Url,
        ParentId = menu.ParentId,
        Order = menu.Order,
        IsDeleted = menu.IsDeleted,
        SubMenus = menu.SubMenus?.Select(m => m.ToResponse()).ToList() ?? new List<MenuResponse>()
    };
}
