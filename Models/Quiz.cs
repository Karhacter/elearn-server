using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace elearn_server.Models
{
    public class Quiz
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Course ID is required")]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        [Required(ErrorMessage = "Quiz Title is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be 3-100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Total marks are required")]
        [Range(1, 100, ErrorMessage = "Marks must be between 1 and 100")]
        public int TotalMarks { get; set; }
    }
}
