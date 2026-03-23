using elearn_server.Application.DTOs;
using elearn_server.Domain.Entities;
using elearn_server.Application.Requests;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/[controller]")]
[Authorize]
public class OrderController(IOrderService orderService) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOrders() => FromResult(await orderService.GetAllAsync());

    [HttpGet("detail/{id}")]
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> GetOrderById(int id) => FromResult(await orderService.GetByIdAsync(id));

    [HttpPost("add")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateOrder([FromBody] Order order) => FromResult(await orderService.CreateBasicAsync(order));

    [HttpPost("create")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest orderRequest) => FromResult(await orderService.CreateAsync(orderRequest));

    [HttpPut("edit/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] OrderUpdateDTO dto) => FromResult(await orderService.UpdateAsync(id, dto));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOrder(int id) => FromResult(await orderService.DeleteAsync(id));

    [HttpGet("tracking/{userId}")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> GetUserOrder(int userId) => FromResult(await orderService.GetByUserIdAsync(userId));
}
