using Microsoft.AspNetCore.Http;

namespace AiDocSummariser.Services;

public record SavedFileResult(string RelativePath, string AbsolutePath);

public interface IFileService
{
    Task<SavedFileResult> SaveFileAsync(IFormFile file);
}