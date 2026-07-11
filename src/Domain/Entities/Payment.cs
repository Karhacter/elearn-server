using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using elearn_server.Domain.Enums;
namespace elearn_server.Domain.Entities
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

        public int? OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        public int? CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        [Required(ErrorMessage = "Payment date is required")]
        [DataType(DataType.DateTime)]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [StringLength(50, ErrorMessage = "Method cannot exceed 50 characters")]
        public string Method { get; set; } = "VNPay";

        [StringLength(50)]
        public string Gateway { get; set; } = "VNPay";

        [StringLength(100)]
        public string? GatewayTransactionNo { get; set; }

        [StringLength(20)]
        public string? GatewayResponseCode { get; set; }

        [StringLength(50)]
        public string? GatewayBankCode { get; set; }

        public DateTime? GatewayPayDate { get; set; }

        [StringLength(100)]
        public string? IdempotencyKey { get; set; }

        public string? RawGatewayPayload { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime? FailedAt { get; set; }

        public DateTime? CanceledAt { get; set; }

        public DateTime? RefundedAt { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    }
}
