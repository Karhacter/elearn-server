using System.ComponentModel.DataAnnotations;
using elearn_server.Domain.Enums;

namespace elearn_server.Domain.Entities;

public class User : BaseEntity
{
    [Key]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Full Name is required")]
    [MaxLength(100, ErrorMessage = "Full Name cannot exceed 100 characters")]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Please enter a valid Phone No")]
    [Required(ErrorMessage = "Phone number is required")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Role is required")]
    [RegularExpression("^(Admin|Instructor|Student)$", ErrorMessage = "Role must be Admin, Instructor, or Student")]
    public string? Role { get; set; }

    [MaxLength(100, ErrorMessage = "Invalid URL format for profile picture")]
    public string? ProfilePicture { get; set; }

    public GenderType? Gender { get; set; }

    public DateTime? Birthday { get; set; }

    [MaxLength(20, ErrorMessage = "Country code cannot exceed 20 characters")]
    public string? CountryCode { get; set; }

    [MaxLength(100, ErrorMessage = "Country name cannot exceed 100 characters")]
    public string? CountryName { get; set; }

    [MaxLength(20, ErrorMessage = "City code cannot exceed 20 characters")]
    public string? CityCode { get; set; }

    [MaxLength(100, ErrorMessage = "City name cannot exceed 100 characters")]
    public string? CityName { get; set; }

    [MaxLength(255, ErrorMessage = "Street cannot exceed 255 characters")]
    public string? Street { get; set; }

    public bool IsDeleted { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }

    public ICollection<Enrollment>? Enrollments { get; set; }
    public ICollection<Payment>? Payments { get; set; }
    public ICollection<Wishlist>? Wishlists { get; set; }
    public ICollection<Certificate>? Certificates { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public ICollection<Rating>? Ratings { get; set; }
    public ICollection<Notification>? Notifications { get; set; }
    public ICollection<RefreshToken>? RefreshTokens { get; set; }
}
