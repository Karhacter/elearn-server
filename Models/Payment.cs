using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace elearn_server.Models
{
    public class Payment
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

        [Required(ErrorMessage = "Payment date is required")]
        [DataType(DataType.DateTime)]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [StringLength(20, ErrorMessage = "Method cannot exceed 20 characters")]
        public string Method { get; set; } = "Credit Card";
    }
}
