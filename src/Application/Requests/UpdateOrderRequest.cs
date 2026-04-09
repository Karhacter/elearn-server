using System.Collections.Generic;
using elearn_server.Domain.Enums;

namespace elearn_server.Application.Requests
{
    public class UpdateOrderRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public int UserId { get; set; }
        public List<OrderItemRequest>? Items { get; set; }
    }
}
