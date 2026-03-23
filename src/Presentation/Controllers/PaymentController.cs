using elearn_server.Application.Requests;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/[controller]")]
public class PaymentController(IPaymentService paymentService) : ApiControllerBase
{
    [HttpGet("{userId}")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> GetUserPayments(int userId) => FromResult(await paymentService.GetByUserIdAsync(userId));

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentCreateRequest request) => FromResult(await paymentService.CreateAsync(request));

    [HttpPost("confirm/{orderId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ConfirmPayment(int orderId) => FromResult(await paymentService.ConfirmPaymentAsync(orderId));
}
