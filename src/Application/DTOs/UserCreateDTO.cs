using System.ComponentModel.DataAnnotations;
using elearn_server.Application.DTOs.Validation;

namespace elearn_server.Application.DTOs
{
    public class UserCreateDTO
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

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Admin|Instructor|Student)$", ErrorMessage = "Role must be Admin, Instructor, or Student")]
        public string Role { get; set; } = string.Empty;

        [Url(ErrorMessage = "Invalid URL format for profile picture")]
        public string? ProfilePicture { get; set; } // Made nullable
    }
}
