using elearn_server.Application.Interfaces;
using elearn_server.Application.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace elearn_server.Presentation.Controllers;

[Route("api/[controller]")]
public class CertificateController(ICertificateService certificateService) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,Instructor,Student")]
    public async Task<IActionResult> GetCertificates() => FromResult(await certificateService.GetAllAsync());

    [HttpGet("eligibility")]
    [Authorize(Roles = "Admin,Instructor,Student")]
    public async Task<IActionResult> CheckEligibility([FromQuery] int userId, [FromQuery] int courseId)
    {
        if (!CanAccessUserScope(userId))
        {
            return Forbid();
        }

        return FromResult(await certificateService.CheckEligibilityAsync(userId, courseId));
    }

    [HttpPost("generate")]
    [Authorize(Roles = "Admin,Instructor,Student")]
    public async Task<IActionResult> GenerateCertificate([FromBody] CertificateGenerateRequest request)
    {
        if (!CanAccessUserScope(request.UserId))
        {
            return Forbid();
        }

        var verificationBaseUrl = $"{Request.Scheme}://{Request.Host}/api/certificate/verify";
        return FromResult(await certificateService.GenerateCertificateAsync(request, verificationBaseUrl));
    }
    
    [HttpGet("verify/{code}")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyCertificate(string code) =>
        FromResult(await certificateService.VerifyCertificateAsync(code));

    [HttpPost]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> IssueCertificate([FromBody] CertificateCreateRequest request) =>
        FromResult(await certificateService.CreateAsync(request));

    private bool CanAccessUserScope(int targetUserId)
    {
        if (User.IsInRole("Admin") || User.IsInRole("Instructor"))
        {
            return true;
        }

        if (!User.IsInRole("Student"))
        {
            return false;
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var currentUserId) && currentUserId == targetUserId;
    }
}
