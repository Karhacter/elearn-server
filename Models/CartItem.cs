using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace elearn_server.Models
{
    public class CartItem : BaseEntity
    {
        [Key]
        public int CartItemId { get; set; }

        [Required]
        public int CartId { get; set; }

        public Cart Cart { get; set; }

        [Required]
        public int CourseID { get; set; }

        public Course Course { get; set; }

        public int Quantity { get; set; }
        public decimal PriceAtTime { get; set; }
    }
}