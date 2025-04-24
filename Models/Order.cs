using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using elearn_server.Models;

namespace elearn_server.Models;

public class Order : BaseEntity
{
    [Key]
    public int OrderID { set; get; }

    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; }

    [MaxLength(10, ErrorMessage = "Phone cannot exceed 10 characters")]
    public string Phone { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [MaxLength(100, ErrorMessage = "Address cannot exceed 100 characters")]
    public string Address { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Required(ErrorMessage = "User ID is required")]
    public int user_id { get; set; }

    public int StatusOrderId { get; set; }

    [ForeignKey("StatusOrderId")]
    public StatusOrder StatusOrder { get; set; }

    // Mối quan hệ với OrderDetail
    public ICollection<OrderDetail> OrderDetails { get; set; }
}