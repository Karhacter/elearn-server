using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Application.Mappings;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;
using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
namespace elearn_server.Infrastructure.Services.Commerce;

public class EnrollmentService(IEnrollmentRepository repository) : IEnrollmentService
{
    public async Task<ServiceResult<IReadOnlyCollection<EnrollmentResponse>>> GetAllAsync() =>
        ServiceResult<IReadOnlyCollection<EnrollmentResponse>>.Ok((await repository.GetAllAsync()).Select(e => e.ToResponse()).ToList());

    public async Task<ServiceResult<EnrollmentResponse>> CreateAsync(EnrollmentCreateRequest request)
    {
        if (await repository.GetUserAsync(request.UserId) is null)
        {
            return ServiceResult<EnrollmentResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }
        var course = await repository.GetCourseAsync(request.CourseId);
        if (course is null)
        {
            return ServiceResult<EnrollmentResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }
        if (await repository.GetByUserAndCourseAsync(request.UserId, request.CourseId) is not null)
        {
            return ServiceResult<EnrollmentResponse>.Fail(StatusCodes.Status409Conflict, "Enrollment already exists.");
        }

        var enrollment = new Enrollment
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            EnrollmentDate = DateTime.UtcNow,
            IsActive = true,
            Course = course
        };

        await repository.AddAsync(enrollment);
        await repository.SaveChangesAsync();
        return ServiceResult<EnrollmentResponse>.Created(enrollment.ToResponse(), "Enrollment created successfully.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var enrollment = await repository.GetByIdAsync(id);
        if (enrollment is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Enrollment not found.");
        }

        repository.Remove(enrollment);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Enrollment deleted successfully.");
    }
}
