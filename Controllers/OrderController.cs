
using elearn_server.Data;
using elearn_server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using elearn_server.Request;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using elearn_server.DTO;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class OrderController : ControllerBase
{
    private readonly AppDbContext _context;
    public OrderController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetOrders()
    {
        var orders = _context.Orders.ToList();
        return Ok(orders);
    }

    // GET: api/category/{id}
    [HttpGet("detail/{id}")]
    public IActionResult GetOrderById(int id)
    {
        var order = _context.Orders.Find(id);
        if (order == null)
            return NotFound();
        return Ok(order);
    }

    // POST: api/order/add
    [HttpPost("add")]
    public IActionResult CreateOrder(Order order)
    {
        order.CreatedAt = DateTime.Now;
        order.UpdatedBy = "system";
        order.UpdatedAt = DateTime.Now;
        if (order == null)
            return BadRequest();

        _context.Orders.Add(order);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderID }, order);
    }


    // create
    [HttpPost("create")]
    public IActionResult CreateOrder([FromBody] OrderRequest orderRequest)
    {
        // Validate required fields
        if (orderRequest == null)
            return BadRequest(new { message = "Order data is required." });

        if (orderRequest.Items == null || !orderRequest.Items.Any())
            return BadRequest(new { message = "Order must contain at least one item." });

        // Create order
        var order = new Order
        {
            Name = orderRequest.Name,
            Email = orderRequest.Email,
            Phone = orderRequest.Phone,
            Address = orderRequest.Address,
            user_id = orderRequest.UserId,
            StatusOrderId = orderRequest.StatusOrderId,
            CreatedAt = DateTime.Now,
            OrderDetails = new List<OrderDetail>()
        };

        // Process items
        foreach (var item in orderRequest.Items)
        {
            var course = _context.Courses.Find(item.CourseID);
            if (course == null)
                return NotFound(new { message = $"Course with ID {item.CourseID} not found." });

            var orderDetail = new OrderDetail
            {
                CourseId = item.CourseID,
                Quantity = item.Quantity,
                Price = course.Price,
                // Set other required fields
            };
            order.OrderDetails.Add(orderDetail);
        }

        _context.Orders.Add(order);
        _context.SaveChanges();

        return Ok(new
        {
            orderId = order.OrderID,
            status = "Order created successfully"
        });
    }
    // PUT: api/category/{id}


    [HttpPut("edit/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] OrderUpdateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderID == id);

        if (order == null)
            return NotFound("Không tìm thấy đơn hàng.");

        order.Name = dto.Name;

        var status = await _context.StatusOrders.FindAsync(dto.StatusOrderId);
        if (status == null)
            return BadRequest($"Không tìm thấy trạng thái đơn hàng với id {dto.StatusOrderId}");

        order.StatusOrderId = dto.StatusOrderId;
        order.UpdatedAt = DateTime.Now;
        order.UpdatedBy = "admin";

        if (dto.Items != null && dto.Items.Any())
        {
            _context.orderDetails.RemoveRange(order.OrderDetails);
            order.OrderDetails = new List<OrderDetail>();
            decimal totalPrice = 0;

            foreach (var item in dto.Items)
            {
                var course = await _context.Courses.FindAsync(item.CourseID);
                if (course == null)
                    return BadRequest($"Không tìm thấy sản phẩm với id {item.CourseID}");

                var priceSale = course.Price - (course.Price * course.Discount / 100);
                var itemTotal = priceSale * item.Quantity;

                var detail = new OrderDetail
                {
                    CourseId = course.CourseId,
                    Quantity = item.Quantity,
                    Price = course.Price,
                };
                order.OrderDetails.Add(detail);
            }
        }

        await _context.SaveChangesAsync();

        return Ok(order); // ✅ Đảm bảo return luôn xảy ra
    }

    // DELETE: api/category/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteOrder(int id)
    {
        var order = _context.Orders.Find(id);
        if (order == null)
            return NotFound();

        _context.Orders.Remove(order);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpGet("tracking/{userId}")]
    public async Task<IActionResult> GetUserOrder(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound("User Chưa Đăng Nhập");

        var orders = await _context.Orders
            .Where(c => c.user_id == userId)
            .Include(o => o.OrderDetails)
            .Include(o => o.StatusOrder)
            .ToListAsync();

        if (orders == null || orders.Count == 0)
            return NotFound("No orders found for this user.");

        return Ok(orders);
    }
}
