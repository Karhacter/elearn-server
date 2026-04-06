using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface IEnrollmentService
{
    Task<ServiceResult<IReadOnlyCollection<EnrollmentResponse>>> GetAllAsync();
    Task<ServiceResult<EnrollmentResponse>> CreateAsync(EnrollmentCreateRequest request);
    Task<ServiceResult<object>> DeleteAsync(int id);
}