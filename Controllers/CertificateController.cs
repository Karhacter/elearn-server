using Microsoft.AspNetCore.Mvc;
using elearn_server.Models;
using elearn_server.Data;


[ApiController]
[Route("api/[controller]")]
public class CertificateController : ControllerBase
{
    private readonly AppDbContext _context;

    public CertificateController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetCertificates() => Ok(_context.Certificates.ToList());

    [HttpPost]
    public IActionResult IssueCertificate(Certificate certificate)
    {
        _context.Certificates.Add(certificate);
        _context.SaveChanges();
        return Ok(certificate);
    }
}
