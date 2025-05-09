
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace elearn_server.Models
{
    public class Category : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Category name must be between 3 and 50 characters")]
        public string? Name { get; set; }

        [StringLength(10000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        // Navigation Property - One category can have many courses
        public ICollection<Course>? Courses { get; set; }
    }
}
