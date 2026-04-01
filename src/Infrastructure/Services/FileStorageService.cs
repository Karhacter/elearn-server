namespace elearn_server.Infrastructure.Services;

public interface IFileStorageService
{
    Task<string> SaveImageAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);
}

public class FileStorageService(IWebHostEnvironment environment) : IFileStorageService
{
    public async Task<string> SaveImageAsync(IFormFile file, CancellationToken cancellationToken = default)
        => await SaveFileAsync(file, "uploads", cancellationToken);

    public async Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
    {
        var webRootPath = environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        var safeFolder = string.IsNullOrWhiteSpace(folder) ? "uploads" : folder.Trim().Replace('/', Path.DirectorySeparatorChar);
        var targetFolder = Path.Combine(webRootPath, safeFolder);
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(targetFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        var folderUrl = safeFolder.Replace('\\', '/').Trim('/');
        return $"/{folderUrl}/{fileName}";
    }
}
