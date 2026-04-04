namespace elearn_server.Application.Responses;

public class ImageUploadResponse
{
    public string ImageUrl { get; set; } = string.Empty;
}

public class BulkSoftDeleteResponse
{
    public int RequestedCount { get; set; }
    public int ProcessedCount { get; set; }
    public int IgnoredCount { get; set; }
}
