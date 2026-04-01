using elearn_server.Domain.Entities;

namespace elearn_server.Domain.Interfaces;

public interface IAssignmentRepository
{
    Task<Assignment?> GetAssignmentByIdAsync(int assignmentId);
    Task<List<Assignment>> GetAssignmentsByCourseIdAsync(int courseId);
    Task<AssignmentSubmission?> GetSubmissionByIdAsync(int submissionId);
    Task<AssignmentSubmission?> GetStudentSubmissionAsync(int assignmentId, int studentId);
    Task<List<AssignmentSubmission>> GetSubmissionsByAssignmentIdAsync(int assignmentId);
    Task<bool> CourseExistsAsync(int courseId);
    Task<bool> IsUserEnrolledAsync(int userId, int courseId);
    Task AddAssignmentAsync(Assignment assignment);
    Task AddSubmissionAsync(AssignmentSubmission submission);
    void RemoveAssignment(Assignment assignment);
    Task SaveChangesAsync();
}
