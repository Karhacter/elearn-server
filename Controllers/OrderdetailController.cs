using elearn_server.Data;
using elearn_server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace elearn_server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderdetailController : ControllerBase
{
    private readonly AppDbContext _context;
    public OrderdetailController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetOrders()
    {
        var orders = _context.orderDetails.ToList();
        return Ok(orders);
    }

    // GET: api/category/{id}
    [HttpGet("{id}")]
    public IActionResult GetOrderById(int id)
    {
        var orderdetail = _context.orderDetails.Find(id);
        if (orderdetail == null)
            return NotFound();
        return Ok(orderdetail);
    }
    [HttpGet("order/{orderId}")]
    public IActionResult GetOrderByOrderId(int orderId)
    {
        var orderdetail = _context.orderDetails.Include(o => o.Order).Include(o => o.Course).Where(o => o.OrderId == orderId).ToList();
        if (orderdetail == null)
            return NotFound($"No OrderDetails found for Order ID {orderdetail}.");
        return Ok(orderdetail);
    }
    // POST: api/category
    [HttpPost]
    public IActionResult CreateOrder(OrderDetail orderDetail)
    {
        orderDetail.CreatedAt = DateTime.Now;
        orderDetail.UpdatedBy = "system";
        orderDetail.UpdatedAt = DateTime.Now;
        if (orderDetail == null)
            return BadRequest();

        _context.orderDetails.Add(orderDetail);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetOrderById), new { id = orderDetail.Id }, orderDetail);
    }

    // PUT: api/category/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateOrder(int id, OrderDetail orderDetail)
    {
        if (id != orderDetail.Id)
            return BadRequest();

        _context.Entry(orderDetail).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _context.SaveChanges();

        return NoContent();
    }

    // DELETE: api/category/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteOrder(int id)
    {
        var category = _context.orderDetails.Find(id);
        if (category == null)
            return NotFound();

        _context.orderDetails.Remove(category);
        _context.SaveChanges();

        return NoContent();
    }
}