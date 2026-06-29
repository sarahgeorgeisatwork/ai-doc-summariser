namespace AiDocSummariser.Services;

public interface IOpenAiService
{
    Task<string> SummarizeFileAsync(string filePath);
}