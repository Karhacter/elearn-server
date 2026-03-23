using elearn_server.Application.Requests;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/[controller]")]
public class CertificateController(ICertificateService certificateService) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,Instructor,Student")]
    public async Task<IActionResult> GetCertificates() => FromResult(await certificateService.GetAllAsync());

    [HttpPost]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> IssueCertificate([FromBody] CertificateCreateRequest request) => FromResult(await certificateService.CreateAsync(request));
}
