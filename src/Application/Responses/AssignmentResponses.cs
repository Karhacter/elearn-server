namespace elearn_server.Application.Responses;

public class AssignmentResponse
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
}

public class AssignmentSubmissionResponse
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? FileUrl { get; set; }
    public string? TextSubmission { get; set; }
    public DateTime SubmittedAt { get; set; }
    public bool IsLate { get; set; }
    public double? Grade { get; set; }
    public string? InstructorFeedback { get; set; }
    public int? GradedByInstructorId { get; set; }
    public DateTime? GradedAt { get; set; }
}
