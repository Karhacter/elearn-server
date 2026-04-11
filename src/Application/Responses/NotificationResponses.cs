namespace elearn_server.Application.Responses;

public class NotificationResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public string? ActionUrl { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class NotificationSummaryResponse
{
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
}
