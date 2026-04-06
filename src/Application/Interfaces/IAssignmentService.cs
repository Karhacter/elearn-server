using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface IAssignmentService
{
    Task<ServiceResult<AssignmentResponse>> CreateAssignmentAsync(AssignmentUpsertRequest request);
    Task<ServiceResult<AssignmentResponse>> UpdateAssignmentAsync(int assignmentId, AssignmentUpsertRequest request);
    Task<ServiceResult<object>> DeleteAssignmentAsync(int assignmentId);
    Task<ServiceResult<IReadOnlyCollection<AssignmentResponse>>> GetAssignmentsByCourseAsync(int courseId);
    Task<ServiceResult<AssignmentSubmissionResponse>> SubmitAssignmentAsync(int assignmentId, int studentId, AssignmentSubmissionRequest request, IFormFile? file, CancellationToken cancellationToken);
    Task<ServiceResult<AssignmentSubmissionResponse>> GetMySubmissionAsync(int assignmentId, int studentId);
    Task<ServiceResult<IReadOnlyCollection<AssignmentSubmissionResponse>>> GetSubmissionsAsync(int assignmentId);
    Task<ServiceResult<AssignmentSubmissionResponse>> GradeSubmissionAsync(int submissionId, int instructorId, AssignmentGradeRequest request);
}