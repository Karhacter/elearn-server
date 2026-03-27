using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Application.Mappings;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence.Repositories;
using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
namespace elearn_server.Infrastructure.Services.Commerce;

public class CertificateService(ICertificateRepository repository) : ICertificateService
{
    public async Task<ServiceResult<IReadOnlyCollection<CertificateResponse>>> GetAllAsync() =>
        ServiceResult<IReadOnlyCollection<CertificateResponse>>.Ok((await repository.GetAllAsync()).Select(c => c.ToResponse()).ToList());

    public async Task<ServiceResult<CertificateResponse>> CreateAsync(CertificateCreateRequest request)
    {
        if (await repository.GetUserAsync(request.UserId) is null)
        {
            return ServiceResult<CertificateResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }
        if (await repository.GetCourseAsync(request.CourseId) is null)
        {
            return ServiceResult<CertificateResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        var certificate = new Certificate
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            CertificateUrl = request.CertificateUrl,
            DateIssued = DateTime.UtcNow
        };

        await repository.AddAsync(certificate);
        await repository.SaveChangesAsync();
        return ServiceResult<CertificateResponse>.Created(certificate.ToResponse(), "Certificate created successfully.");
    }
}
