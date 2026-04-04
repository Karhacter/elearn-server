using System.ComponentModel.DataAnnotations;
using elearn_server.Application.DTOs.Validation;

namespace elearn_server.Application.DTOs;

public class RegisterUserDTO : IValidatableObject
{
    [Required(ErrorMessage = "Full Name is required")]
    [MinLength(2, ErrorMessage = "Full Name must be at least 2 characters long")]
    [MaxLength(100, ErrorMessage = "Full Name cannot exceed 100 characters")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Full Name cannot be blank")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Please enter a valid Phone No")]
    [Required(ErrorMessage = "Phone number is required")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Phone number cannot be blank")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [MaxLength(255, ErrorMessage = "Password cannot exceed 255 characters")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Password cannot be blank")]
    [StrongPassword]
    public string Password { get; set; } = string.Empty;

    [Url(ErrorMessage = "Invalid URL format for profile picture")]
    public string? ProfilePicture { get; set; }

    public string? Gender { get; set; }

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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Birthday.HasValue && Birthday.Value.Date >= DateTime.UtcNow.Date)
        {
            yield return new ValidationResult("Birthday must be in the past.", [nameof(Birthday)]);
        }

        if (!string.IsNullOrWhiteSpace(Gender) &&
            Gender is not ("Male" or "Female" or "PreferNotToSay"))
        {
            yield return new ValidationResult("Gender must be Male, Female, or PreferNotToSay.", [nameof(Gender)]);
        }
    }
}
