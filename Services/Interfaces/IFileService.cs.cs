using Microsoft.AspNetCore.Http;
using Sofia.Web.Models;

namespace Sofia.Web.Services;

public interface IFileService
{
    Task<MediaFile> SaveAsync(IFormFile file, CancellationToken cancellationToken = default);
}
