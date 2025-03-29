using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace elearn_server.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required(ErrorMessage = "Course ID is required")]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        [Required(ErrorMessage = "Comment is required")]
        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        public string Content { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CommentDate { get; set; } = DateTime.UtcNow;
    }
}
