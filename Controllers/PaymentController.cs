using Microsoft.AspNetCore.Mvc;
using elearn_server.Models;
using elearn_server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class PaymentController : ControllerBase
{
    private readonly AppDbContext _context;

    public PaymentController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public IActionResult GetUserPayments(int userId)
    {
        var payments = _context.Payments.Where(p => p.UserId == userId).ToList();
        return Ok(payments);
    }

    [HttpPost]
    public IActionResult ProcessPayment(Payment payment)
    {
        _context.Payments.Add(payment);
        _context.SaveChanges();
        return Ok(payment);
    }
    // checkout

    [HttpPost("confirm/{orderId}")]
    public IActionResult ConfirmPayment(int orderId)
    {
        // Lấy đơn hàng từ database
        var order = _context.Orders
            .Include(o => o.StatusOrder) // Bao gồm trạng thái của đơn hàng
            .FirstOrDefault(o => o.OrderID == orderId);

        // Kiểm tra xem đơn hàng có tồn tại không
        if (order == null)
            return NotFound(new { message = "Đơn hàng không tồn tại." });

        // Kiểm tra xem trạng thái thanh toán của đơn hàng đã là "Đã thanh toán" chưa
        if (order.StatusOrder.Name == "Đã thanh toán")
            return BadRequest(new { message = "Đơn hàng đã được thanh toán." });

        // Lấy trạng thái "Đã thanh toán" từ bảng StatusOrders
        var paidStatus = _context.StatusOrders
            .FirstOrDefault(s => s.Name == "Đã thanh toán");

        if (paidStatus == null)
            return BadRequest(new { message = "Không tìm thấy trạng thái thanh toán." });

        try
        {
            // Cập nhật trạng thái đơn hàng sang "Đã thanh toán"
            order.StatusOrderId = paidStatus.Id;

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            // Trả về thông báo thành công
            return Ok(new { message = "Thanh toán thành công. Đơn hàng đã được hoàn tất.", orderId = order.OrderID });
        }
        catch (Exception ex)
        {
            // Nếu có lỗi xảy ra, trả về thông báo lỗi
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật đơn hàng.", details = ex.Message });
        }
    }
}
