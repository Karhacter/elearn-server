using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using elearn_server.Domain.Enums;
namespace elearn_server.Domain.Entities;

public class Order : BaseEntity
{
    [Key]
    public int OrderID { set; get; }

    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string? Name { get; set; }

    [MaxLength(10, ErrorMessage = "Phone cannot exceed 10 characters")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [MaxLength(100, ErrorMessage = "Address cannot exceed 100 characters")]
    public string? Address { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Required(ErrorMessage = "User ID is required")]
    public int user_id { get; set; }

    public ICollection<OrderDetail>? OrderDetails { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}
