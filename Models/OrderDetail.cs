using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace elearn_server.Models;

public class OrderDetail : BaseEntity
{
    [Key]
    public int Id { get; set; }
    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal Amount { get; set; }

    [ForeignKey("OrderId")]
    public Order? Order { get; set; }

    [Required(ErrorMessage = "Order id is required")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "Course id is required")]
    public int CourseId { get; set; }

    [ForeignKey("CourseId")]
    public Course? Course { get; set; }
}
