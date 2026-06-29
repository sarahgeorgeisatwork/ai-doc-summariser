namespace AiDocSummariser.Models;

public class Document
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}