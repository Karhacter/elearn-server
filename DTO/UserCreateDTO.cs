using System.ComponentModel.DataAnnotations;

namespace elearn_server.DTO
{
    public class UserCreateDTO
    {
        [Required(ErrorMessage = "Full Name is required")]
        [MaxLength(100, ErrorMessage = "Full Name cannot exceed 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Please enter a valid Phone No")]
        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Admin|Instructor|Student)$", ErrorMessage = "Role must be Admin, Instructor, or Student")]
        public string Role { get; set; }

        [Url(ErrorMessage = "Invalid URL format for profile picture")]
        public string? ProfilePicture { get; set; } // Made nullable
    }
}
