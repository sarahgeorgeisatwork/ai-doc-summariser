namespace AiDocSummariser.Models;

public class Summary
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}