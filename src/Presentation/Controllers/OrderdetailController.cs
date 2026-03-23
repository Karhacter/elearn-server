using elearn_server.Application.Requests;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class OrderdetailController(IOrderDetailService orderDetailService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOrders() => FromResult(await orderDetailService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id) => FromResult(await orderDetailService.GetByIdAsync(id));

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetOrderByOrderId(int orderId) => FromResult(await orderDetailService.GetByOrderIdAsync(orderId));

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderDetailUpsertRequest request) => FromResult(await orderDetailService.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDetailUpsertRequest request) => FromResult(await orderDetailService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id) => FromResult(await orderDetailService.DeleteAsync(id));
}
