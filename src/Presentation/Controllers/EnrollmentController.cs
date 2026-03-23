using elearn_server.Application.Requests;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/[controller]")]
public class EnrollmentController(IEnrollmentService enrollmentService) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> GetAllEnrollments() => FromResult(await enrollmentService.GetAllAsync());

    [HttpPost]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> EnrollUser([FromBody] EnrollmentCreateRequest request) => FromResult(await enrollmentService.CreateAsync(request));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> UnenrollUser(int id) => FromResult(await enrollmentService.DeleteAsync(id));
}
