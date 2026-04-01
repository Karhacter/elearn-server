using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Domain.Interfaces;
using elearn_server.Infrastructure.Services;

namespace elearn_server.Infrastructure.Services.Core.Assignments;

public class AssignmentService(IAssignmentRepository repository, IFileStorageService fileStorageService) : IAssignmentService
{
    public async Task<ServiceResult<AssignmentResponse>> CreateAssignmentAsync(AssignmentUpsertRequest request)
    {
        if (!await repository.CourseExistsAsync(request.CourseId))
        {
            return ServiceResult<AssignmentResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        if (request.DueDate <= DateTime.UtcNow)
        {
            return ServiceResult<AssignmentResponse>.Fail(StatusCodes.Status400BadRequest, "Due date must be in the future.");
        }

        var assignment = new Assignment
        {
            CourseId = request.CourseId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.AddAssignmentAsync(assignment);
        await repository.SaveChangesAsync();
        return ServiceResult<AssignmentResponse>.Created(ToAssignmentResponse(assignment), "Assignment created successfully.");
    }

    public async Task<ServiceResult<AssignmentResponse>> UpdateAssignmentAsync(int assignmentId, AssignmentUpsertRequest request)
    {
        var assignment = await repository.GetAssignmentByIdAsync(assignmentId);
        if (assignment is null)
        {
            return ServiceResult<AssignmentResponse>.Fail(StatusCodes.Status404NotFound, "Assignment not found.");
        }

        if (!await repository.CourseExistsAsync(request.CourseId))
        {
            return ServiceResult<AssignmentResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        assignment.CourseId = request.CourseId;
        assignment.Title = request.Title.Trim();
        assignment.Description = request.Description.Trim();
        assignment.DueDate = request.DueDate;
        assignment.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync();
        return ServiceResult<AssignmentResponse>.Ok(ToAssignmentResponse(assignment), "Assignment updated successfully.");
    }

    public async Task<ServiceResult<object>> DeleteAssignmentAsync(int assignmentId)
    {
        var assignment = await repository.GetAssignmentByIdAsync(assignmentId);
        if (assignment is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Assignment not found.");
        }

        repository.RemoveAssignment(assignment);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Assignment deleted successfully.");
    }

    public async Task<ServiceResult<IReadOnlyCollection<AssignmentResponse>>> GetAssignmentsByCourseAsync(int courseId)
    {
        var assignments = await repository.GetAssignmentsByCourseIdAsync(courseId);
        return ServiceResult<IReadOnlyCollection<AssignmentResponse>>.Ok(assignments.Select(ToAssignmentResponse).ToList());
    }

    public async Task<ServiceResult<AssignmentSubmissionResponse>> SubmitAssignmentAsync(int assignmentId, int studentId, AssignmentSubmissionRequest request, IFormFile? file, CancellationToken cancellationToken)
    {
        var assignment = await repository.GetAssignmentByIdAsync(assignmentId);
        if (assignment is null)
        {
            return ServiceResult<AssignmentSubmissionResponse>.Fail(StatusCodes.Status404NotFound, "Assignment not found.");
        }

        if (!await repository.IsUserEnrolledAsync(studentId, assignment.CourseId))
        {
            return ServiceResult<AssignmentSubmissionResponse>.Fail(StatusCodes.Status403Forbidden, "You are not enrolled in this course.");
        }

        var textSubmission = request.TextSubmission?.Trim();
        if ((file is null || file.Length == 0) && string.IsNullOrWhiteSpace(textSubmission))
        {
            return ServiceResult<AssignmentSubmissionResponse>.Fail(StatusCodes.Status400BadRequest, "Text submission or file upload is required.");
        }

        var fileUrl = file is { Length: > 0 }
            ? await fileStorageService.SaveFileAsync(file, "uploads/assignments", cancellationToken)
            : null;

        var submission = await repository.GetStudentSubmissionAsync(assignmentId, studentId);
        if (submission is null)
        {
            submission = new AssignmentSubmission
            {
                AssignmentId = assignmentId,
                StudentId = studentId,
                TextSubmission = textSubmission,
                FileUrl = fileUrl,
                SubmittedAt = DateTime.UtcNow,
                IsLate = DateTime.UtcNow > assignment.DueDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await repository.AddSubmissionAsync(submission);
        }
        else
        {
            submission.TextSubmission = textSubmission ?? submission.TextSubmission;
            submission.FileUrl = fileUrl ?? submission.FileUrl;
            submission.SubmittedAt = DateTime.UtcNow;
            submission.IsLate = DateTime.UtcNow > assignment.DueDate;
            submission.UpdatedAt = DateTime.UtcNow;
        }

        await repository.SaveChangesAsync();
        var saved = await repository.GetStudentSubmissionAsync(assignmentId, studentId);
        return ServiceResult<AssignmentSubmissionResponse>.Ok(ToSubmissionResponse(saved!), "Assignment submitted successfully.");
    }

    public async Task<ServiceResult<AssignmentSubmissionResponse>> GetMySubmissionAsync(int assignmentId, int studentId)
    {
        var submission = await repository.GetStudentSubmissionAsync(assignmentId, studentId);
        if (submission is null)
        {
            return ServiceResult<AssignmentSubmissionResponse>.Fail(StatusCodes.Status404NotFound, "Submission not found.");
        }

        return ServiceResult<AssignmentSubmissionResponse>.Ok(ToSubmissionResponse(submission));
    }

    public async Task<ServiceResult<IReadOnlyCollection<AssignmentSubmissionResponse>>> GetSubmissionsAsync(int assignmentId)
    {
        var assignment = await repository.GetAssignmentByIdAsync(assignmentId);
        if (assignment is null)
        {
            return ServiceResult<IReadOnlyCollection<AssignmentSubmissionResponse>>.Fail(StatusCodes.Status404NotFound, "Assignment not found.");
        }

        var submissions = await repository.GetSubmissionsByAssignmentIdAsync(assignmentId);
        return ServiceResult<IReadOnlyCollection<AssignmentSubmissionResponse>>.Ok(submissions.Select(ToSubmissionResponse).ToList());
    }

    public async Task<ServiceResult<AssignmentSubmissionResponse>> GradeSubmissionAsync(int submissionId, int instructorId, AssignmentGradeRequest request)
    {
        var submission = await repository.GetSubmissionByIdAsync(submissionId);
        if (submission is null)
        {
            return ServiceResult<AssignmentSubmissionResponse>.Fail(StatusCodes.Status404NotFound, "Submission not found.");
        }

        submission.Grade = request.Grade;
        submission.InstructorFeedback = request.InstructorFeedback?.Trim();
        submission.GradedByInstructorId = instructorId;
        submission.GradedAt = DateTime.UtcNow;
        submission.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync();
        return ServiceResult<AssignmentSubmissionResponse>.Ok(ToSubmissionResponse(submission), "Submission graded successfully.");
    }

    private static AssignmentResponse ToAssignmentResponse(Assignment assignment) => new()
    {
        Id = assignment.Id,
        CourseId = assignment.CourseId,
        Title = assignment.Title,
        Description = assignment.Description,
        DueDate = assignment.DueDate
    };

    private static AssignmentSubmissionResponse ToSubmissionResponse(AssignmentSubmission submission) => new()
    {
        Id = submission.Id,
        AssignmentId = submission.AssignmentId,
        StudentId = submission.StudentId,
        StudentName = submission.Student?.FullName,
        FileUrl = submission.FileUrl,
        TextSubmission = submission.TextSubmission,
        SubmittedAt = submission.SubmittedAt,
        IsLate = submission.IsLate,
        Grade = submission.Grade,
        InstructorFeedback = submission.InstructorFeedback,
        GradedByInstructorId = submission.GradedByInstructorId,
        GradedAt = submission.GradedAt
    };
}
