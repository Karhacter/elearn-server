using Microsoft.AspNetCore.Mvc;
using elearn_server.Models;
using elearn_server.Data;


[ApiController]
[Route("api/[controller]")]
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
}
