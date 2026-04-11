using System.ComponentModel.DataAnnotations;
using elearn_server.Domain.Enums;

namespace elearn_server.Application.Requests;

public class CreateNotificationRequest
{
    [Required(ErrorMessage = "UserId is required")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [MaxLength(150, ErrorMessage = "Title cannot exceed 150 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Message is required")]
    [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; } = NotificationType.System;

    [MaxLength(500)]
    public string? ActionUrl { get; set; }
}
