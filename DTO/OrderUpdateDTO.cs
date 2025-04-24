using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using elearn_server.Request;

namespace elearn_server.DTO
{
    public class OrderUpdateDTO
    {
        public string Name { get; set; }

        [Required(ErrorMessage = "The StatusOrderId field is required.")]
        [JsonPropertyName("StatusOrder")]
        public int StatusOrderId { get; set; } // sửa từ string Status → int StatusOrderId

        [Required(ErrorMessage = "The Items field is required.")]
        [MinLength(1, ErrorMessage = "The Items field must contain at least one item.")]
        [JsonPropertyName("orderDetails")]
        public List<OrderItemRequest> Items { get; set; }
    }
}
