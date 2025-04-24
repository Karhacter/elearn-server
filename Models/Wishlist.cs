using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace elearn_server.Models
{
    public class Wishlist : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Course ID is required")]
        public int CourseId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    }
}
