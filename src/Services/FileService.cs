using Microsoft.AspNetCore.Http;

namespace AiDocSummariser.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly string _uploads;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
        _uploads = Path.Combine(_env.ContentRootPath, "uploads");
        Directory.CreateDirectory(_uploads);
    }

    public async Task<SavedFileResult> SaveFileAsync(IFormFile file)
    {
        var name = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var abs = Path.Combine(_uploads, name);
        using var fs = new FileStream(abs, FileMode.Create);
        await file.CopyToAsync(fs);
        var rel = Path.Combine("uploads", name).Replace("\\", "/");
        return new SavedFileResult(rel, abs);
    }
}