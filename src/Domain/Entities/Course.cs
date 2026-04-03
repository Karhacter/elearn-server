using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using elearn_server.Domain.Enums;

namespace elearn_server.Domain.Entities;

public class Course : BaseEntity
{
    [Key]
    public int CourseId { get; set; }

    [Required(ErrorMessage = "Course title is required")]
    [MaxLength(150, ErrorMessage = "Title cannot exceed 150 characters")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Course description is required")]
    [MinLength(20, ErrorMessage = "Description must be at least 20 characters")]
    public string? Description { get; set; }

    [ForeignKey("GenreId")]
    public Category? Genre { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }


    [Range(0, double.MaxValue)]
    public decimal Discount { get; set; }


    [Required(ErrorMessage = "Category is required")]
    public int GenreId { get; set; }

    [MaxLength(100, ErrorMessage = "Image cannot exceed 100 characters")]
    public string? Image { get; set; }

    [Range(10, 10000, ErrorMessage = "Duration must be between 10 and 10000 minutes")]
    public int Duration { get; set; }

    [Url(ErrorMessage = "Invalid thumbnail URL format")]
    public string? Thumbnail { get; set; }


    [ForeignKey("User")]
    [Required(ErrorMessage = "Instructor ID is required")]
    public int InstructorId { get; set; }
    public User? Instructor { get; set; }

    [MaxLength(200)]
    public string? Slug { get; set; }

    public CourseStatus Status { get; set; } = CourseStatus.Draft; // Mặc định là Draft
    public bool IsSequential { get; set; }
    public bool IsDeleted { get; set; }
    // One - to - Many || Relationships and Constraints
    public ICollection<Enrollment>? Enrollments { get; set; }      // User's course enrollments
    public ICollection<Payment>? Payments { get; set; }      // User's course enrollments
    public ICollection<Certificate>? Certificates { get; set; }
    public ICollection<Quiz>? Quizzes { get; set; }
    public ICollection<Assignment>? Assignments { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public ICollection<Wishlist>? Wishlists { get; set; }
    public ICollection<Lesson>? Lessons { get; set; }
    public ICollection<CourseSection>? Sections { get; set; }
    public ICollection<LearningOutcome>? LearningOutcomes { get; set; }
    public ICollection<CourseRequirement>? Requirements { get; set; }
    public ICollection<CourseTargetAudience>? TargetAudiences { get; set; }
    public ICollection<Rating>? Ratings { get; set; }
}
