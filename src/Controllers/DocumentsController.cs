using Microsoft.AspNetCore.Mvc;
using AiDocSummariser.Data;
using AiDocSummariser.Models;
using AiDocSummariser.Services;
using Microsoft.EntityFrameworkCore;

namespace AiDocSummariser.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IFileService _files;

    public DocumentsController(AppDbContext db, IFileService files)
    {
        _db = db;
        _files = files;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

        var saved = await _files.SaveFileAsync(file);

        var doc = new Document
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            Path = saved.RelativePath,
            UploadedAt = DateTime.UtcNow
        };

        _db.Documents.Add(doc);
        await _db.SaveChangesAsync();

        return Accepted(new { doc.Id, message = "File accepted. Summary will be generated asynchronously."});
    }

    [HttpGet("{id}/summary")]
    public async Task<IActionResult> GetSummary(int id)
    {
        var summary = await _db.Summaries.FirstOrDefaultAsync(s => s.DocumentId == id);
        if (summary == null) return NotFound();
        return Ok(new { summary.Id, summary.Text, summary.CreatedAt });
    }
}