using elearn_server.Domain.Entities;
using elearn_server.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class AssignmentRepository(AppDbContext context) : IAssignmentRepository
{
    public Task<Assignment?> GetAssignmentByIdAsync(int assignmentId) =>
        context.Assignments.SingleOrDefaultAsync(a => a.Id == assignmentId);

    public Task<List<Assignment>> GetAssignmentsByCourseIdAsync(int courseId) =>
        context.Assignments.Where(a => a.CourseId == courseId).OrderBy(a => a.DueDate).ToListAsync();

    public Task<AssignmentSubmission?> GetSubmissionByIdAsync(int submissionId) =>
        context.AssignmentSubmissions
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .SingleOrDefaultAsync(s => s.Id == submissionId);

    public Task<AssignmentSubmission?> GetStudentSubmissionAsync(int assignmentId, int studentId) =>
        context.AssignmentSubmissions
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .SingleOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);

    public Task<List<AssignmentSubmission>> GetSubmissionsByAssignmentIdAsync(int assignmentId) =>
        context.AssignmentSubmissions
            .Include(s => s.Student)
            .Where(s => s.AssignmentId == assignmentId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();

    public Task<bool> CourseExistsAsync(int courseId) => context.Courses.AnyAsync(c => c.CourseId == courseId);
    public Task<bool> IsUserEnrolledAsync(int userId, int courseId) => context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
    public Task AddAssignmentAsync(Assignment assignment) => context.Assignments.AddAsync(assignment).AsTask();
    public Task AddSubmissionAsync(AssignmentSubmission submission) => context.AssignmentSubmissions.AddAsync(submission).AsTask();
    public void RemoveAssignment(Assignment assignment) => context.Assignments.Remove(assignment);
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
