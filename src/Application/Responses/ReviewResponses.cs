namespace elearn_server.Application.Responses;

public class CommentResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CommentDate { get; set; }
}

public class ReviewResponse
{
    public int RatingId { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public int CourseId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ReplyContent { get; set; }
    public DateTime? ReplyTimestamp { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CourseRatingSummaryResponse
{
    public int CourseId { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int OneStarCount { get; set; }
    public int TwoStarCount { get; set; }
    public int ThreeStarCount { get; set; }
    public int FourStarCount { get; set; }
    public int FiveStarCount { get; set; }
}
