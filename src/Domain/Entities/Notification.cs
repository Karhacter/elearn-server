using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using elearn_server.Domain.Enums;

namespace elearn_server.Domain.Entities;

public class Notification : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [MaxLength(150, ErrorMessage = "Title cannot exceed 150 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Message is required")]
    [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; } = NotificationType.System;

    public bool IsRead { get; set; } = false;

    /// <summary>
    /// Optional deep-link URL the client can navigate to when the notification is tapped.
    /// </summary>
    [MaxLength(500)]
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Soft-delete flag — inherited DeletedAt/DeletedBy from BaseEntity
    /// are used to determine whether the record is logically deleted.
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}
