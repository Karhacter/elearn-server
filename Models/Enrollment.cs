using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace elearn_server.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }
}
