using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace elearn_server.Models;

public class Rating
{
    [Key]
    public int RatingId { get; set; }

    [ForeignKey("Course")]
    [Required(ErrorMessage = "Course ID is required")]
    public int CourseId { get; set; }
    public Course Course { get; set; }

    [ForeignKey("User")]
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }
    public User User { get; set; }

    [Range(1, 5, ErrorMessage = "Score must be between 1 and 5")]
    public int Score { get; set; }

    [MaxLength(500, ErrorMessage = "Review cannot exceed 500 characters")]
    public string Review { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
