using System.ComponentModel.DataAnnotations;

namespace elearn_server.Application.Requests;

public class CategoryUpsertRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }
}

public class CourseUpsertRequest
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MinLength(20)]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Discount { get; set; }

    [Required]
    public int GenreId { get; set; }

    [Range(10, 10000)]
    public int Duration { get; set; }

    [Url]
    public string? Thumbnail { get; set; }

    public string? Image { get; set; }

    [Required]
    public int InstructorId { get; set; }
}

public class UserUpdateRequest
{
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Admin|Instructor|Student)$")]
    public string Role { get; set; } = string.Empty;

    [Url]
    public string? ProfilePicture { get; set; }
}

public class UpdateImageRequest
{
    [Required]
    public string ImageUrl { get; set; } = string.Empty;
}

public class WishlistCreateRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int CourseId { get; set; }
}

public class PaymentCreateRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public double Amount { get; set; }

    [Required]
    [MaxLength(20)]
    public string Method { get; set; } = string.Empty;
}

public class EnrollmentCreateRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int CourseId { get; set; }
}

public class CommentCreateRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;
}

public class CertificateCreateRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    [Url]
    public string CertificateUrl { get; set; } = string.Empty;
}

public class OrderDetailUpsertRequest
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
