namespace elearn_server.Application.Responses;

public class CertificateResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string CertificateUrl { get; set; } = string.Empty;
    public string? VerificationCode { get; set; }
    public DateTime DateIssued { get; set; }
}

public class CertificateEligibilityResponse
{
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public bool IsEligible { get; set; }
    public bool HasCompletedRequiredLessons { get; set; }
    public bool HasPassedFinalQuiz { get; set; }
    public bool HasAttendanceViolation { get; set; }
    public int CompletedLessons { get; set; }
    public int TotalRequiredLessons { get; set; }
    public int? FinalQuizId { get; set; }
}

public class CertificateVerificationResponse
{
    public int CertificateId { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string VerificationCode { get; set; } = string.Empty;
    public string CertificateUrl { get; set; } = string.Empty;
    public DateTime DateIssued { get; set; }
}
