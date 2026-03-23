namespace elearn_server.Application.Responses;

public class AuthenticatedUserResponse
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
}

public class LoginResponse
{
    public AuthenticatedUserResponse User { get; set; } = new();
    public DateTime ExpiresAtUtc { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class AuthCheckResponse
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class CategoryResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class CourseResponse
{
    public int CourseId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public int GenreId { get; set; }
    public string? GenreName { get; set; }
    public string? Image { get; set; }
    public string? Thumbnail { get; set; }
    public int Duration { get; set; }
    public int InstructorId { get; set; }
    public string? InstructorName { get; set; }
}

public class OrderDetailResponse
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
}

public class OrderResponse
{
    public int OrderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int StatusOrderId { get; set; }
    public string? StatusName { get; set; }
    public IReadOnlyCollection<OrderDetailResponse> Items { get; set; } = Array.Empty<OrderDetailResponse>();
}

public class WishlistResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public DateTime AddedDate { get; set; }
}

public class PaymentResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public double Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
}

public class EnrollmentResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public bool IsActive { get; set; }
}

public class CommentResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CommentDate { get; set; }
}

public class CertificateResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string CertificateUrl { get; set; } = string.Empty;
    public DateTime DateIssued { get; set; }
}

public class ImageUploadResponse
{
    public string ImageUrl { get; set; } = string.Empty;
}
