
using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface ICertificateService
{
    Task<ServiceResult<IReadOnlyCollection<CertificateResponse>>> GetAllAsync();
    Task<ServiceResult<CertificateEligibilityResponse>> CheckEligibilityAsync(int userId, int courseId);
    Task<ServiceResult<CertificateResponse>> GenerateCertificateAsync(CertificateGenerateRequest request, string verificationBaseUrl);
    Task<ServiceResult<CertificateVerificationResponse>> VerifyCertificateAsync(string code);
    Task<ServiceResult<CertificateResponse>> CreateAsync(CertificateCreateRequest request);
}

