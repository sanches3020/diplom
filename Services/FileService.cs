using Microsoft.AspNetCore.Hosting;
using Sofia.Web.Data;
using Sofia.Web.Models;

namespace Sofia.Web.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly SofiaDbContext _db;

    private static readonly string[] AllowedExtensions =
        [".jpg", ".jpeg", ".png", ".gif", ".mp4"];

    private const long MaxSizeBytes = 10 * 1024 * 1024; // 10 MB

    public FileService(IWebHostEnvironment env, SofiaDbContext db)
    {
        _env = env;
        _db = db;
    }

    public async Task<MediaFile> SaveAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0 || file.Length > MaxSizeBytes)
            throw new InvalidOperationException("Недопустимый размер файла.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            throw new InvalidOperationException("Недопустимый тип файла.");

        var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsRoot);

        var id = Guid.NewGuid();
        var storedFileName = id + ext;
        var fullPath = Path.Combine(uploadsRoot, storedFileName);

        await using (var stream = File.Create(fullPath))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var media = new MediaFile
        {
            Id = id,
            FileName = file.FileName,
            FilePath = $"uploads/{storedFileName}",
            ContentType = file.ContentType,
            UploadDate = DateTime.UtcNow
        };

        _db.MediaFiles.Add(media);
        await _db.SaveChangesAsync(cancellationToken);

        return media;
    }
}
