using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using elearn_server.Domain.Enums;

namespace elearn_server.Domain.Entities;

public class Rating
{
    [Key]
    public int RatingId { get; set; }

    [ForeignKey("Course")]
    [Required(ErrorMessage = "Course ID is required")]
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    [ForeignKey("User")]
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Range(1, 5, ErrorMessage = "Score must be between 1 and 5")]
    public int Score { get; set; }

    [MaxLength(500, ErrorMessage = "Review cannot exceed 500 characters")]
    public string? Review { get; set; }

    public ReviewStatus Status { get; set; } = ReviewStatus.Pending;

    [MaxLength(1000, ErrorMessage = "Reply cannot exceed 1000 characters")]
    public string? ReplyContent { get; set; }

    public DateTime? ReplyTimestamp { get; set; }

    public Comment? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
