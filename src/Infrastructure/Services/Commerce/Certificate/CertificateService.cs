using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Mappings;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;

namespace elearn_server.Infrastructure.Services.Commerce;

public class CertificateService(ICertificateRepository repository) : ICertificateService
{
    public async Task<ServiceResult<IReadOnlyCollection<CertificateResponse>>> GetAllAsync() =>
        ServiceResult<IReadOnlyCollection<CertificateResponse>>.Ok((await repository.GetAllAsync()).Select(c => c.ToResponse()).ToList());

    public async Task<ServiceResult<CertificateEligibilityResponse>> CheckEligibilityAsync(int userId, int courseId)
    {
        if (await repository.GetUserAsync(userId) is null)
        {
            return ServiceResult<CertificateEligibilityResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }

        if (await repository.GetCourseAsync(courseId) is null)
        {
            return ServiceResult<CertificateEligibilityResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        var isEnrolled = await repository.IsUserEnrolledAsync(userId, courseId);
        var totalLessons = await repository.GetTotalLessonsByCourseAsync(courseId);
        var completedLessons = await repository.GetCompletedLessonsByUserAndCourseAsync(userId, courseId);
        var hasCompletedRequiredLessons = totalLessons == 0 || completedLessons >= totalLessons;

        var finalQuizId = await repository.GetFinalQuizIdAsync(courseId);
        var hasPassedFinalQuiz = !finalQuizId.HasValue || await repository.HasPassedQuizAsync(userId, finalQuizId.Value);

        // Attendance is not tracked in current data model, so this condition is treated as pass.
        var hasAttendanceViolation = false;

        var isEligible = isEnrolled && hasCompletedRequiredLessons && hasPassedFinalQuiz && !hasAttendanceViolation;
        var response = new CertificateEligibilityResponse
        {
            UserId = userId,
            CourseId = courseId,
            IsEligible = isEligible,
            HasCompletedRequiredLessons = hasCompletedRequiredLessons,
            HasPassedFinalQuiz = hasPassedFinalQuiz,
            HasAttendanceViolation = hasAttendanceViolation,
            CompletedLessons = completedLessons,
            TotalRequiredLessons = totalLessons,
            FinalQuizId = finalQuizId
        };

        return ServiceResult<CertificateEligibilityResponse>.Ok(response);
    }

    public async Task<ServiceResult<CertificateResponse>> GenerateCertificateAsync(CertificateGenerateRequest request, string verificationBaseUrl)
    {
        var eligibilityResult = await CheckEligibilityAsync(request.UserId, request.CourseId);
        if (!eligibilityResult.Success || eligibilityResult.Data is null)
        {
            return ServiceResult<CertificateResponse>.Fail(eligibilityResult.StatusCode, eligibilityResult.Message, eligibilityResult.Errors);
        }

        if (!eligibilityResult.Data.IsEligible)
        {
            return ServiceResult<CertificateResponse>.Fail(StatusCodes.Status400BadRequest, "User is not eligible for certificate.");
        }

        if (await repository.GetByUserAndCourseAsync(request.UserId, request.CourseId) is not null)
        {
            return ServiceResult<CertificateResponse>.Fail(StatusCodes.Status409Conflict, "Certificate already exists for this user and course.");
        }

        var verificationCode = await GenerateUniqueVerificationCodeAsync();
        var certificate = new Certificate
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            VerificationCode = verificationCode,
            CertificateUrl = $"{verificationBaseUrl.TrimEnd('/')}/{verificationCode}",
            DateIssued = DateTime.UtcNow
        };

        await repository.AddAsync(certificate);
        await repository.SaveChangesAsync();
        return ServiceResult<CertificateResponse>.Created(certificate.ToResponse(), "Certificate generated successfully.");
    }

    public async Task<ServiceResult<CertificateVerificationResponse>> VerifyCertificateAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return ServiceResult<CertificateVerificationResponse>.Fail(StatusCodes.Status400BadRequest, "Verification code is required.");
        }

        var normalizedCode = code.Trim();
        var certificate = await repository.GetByVerificationCodeAsync(normalizedCode);
        if (certificate is null)
        {
            return ServiceResult<CertificateVerificationResponse>.Fail(StatusCodes.Status404NotFound, "Certificate not found.");
        }

        return ServiceResult<CertificateVerificationResponse>.Ok(new CertificateVerificationResponse
        {
            CertificateId = certificate.Id,
            UserId = certificate.UserId,
            CourseId = certificate.CourseId,
            VerificationCode = certificate.VerificationCode ?? string.Empty,
            CertificateUrl = certificate.CertificateUrl,
            DateIssued = certificate.DateIssued
        });
    }

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

        if (await repository.GetByUserAndCourseAsync(request.UserId, request.CourseId) is not null)
        {
            return ServiceResult<CertificateResponse>.Fail(StatusCodes.Status409Conflict, "Certificate already exists for this user and course.");
        }

        var verificationCode = await GenerateUniqueVerificationCodeAsync();
        var certificate = new Certificate
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            CertificateUrl = request.CertificateUrl,
            VerificationCode = verificationCode,
            DateIssued = DateTime.UtcNow
        };

        await repository.AddAsync(certificate);
        await repository.SaveChangesAsync();
        return ServiceResult<CertificateResponse>.Created(certificate.ToResponse(), "Certificate created successfully.");
    }

    private async Task<string> GenerateUniqueVerificationCodeAsync()
    {
        for (var i = 0; i < 5; i++)
        {
            var code = Guid.NewGuid().ToString("N")[..16].ToUpperInvariant();
            if (!await repository.VerificationCodeExistsAsync(code))
            {
                return code;
            }
        }

        return $"{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}
