using Azure.AI.OpenAI;

namespace AiDocSummariser.Services;

public class OpenAiService : IOpenAiService
{
    private readonly IConfiguration _cfg;
    private readonly ILogger<OpenAiService> _log;

    public OpenAiService(IConfiguration cfg, ILogger<OpenAiService> log)
    {
        _cfg = cfg;
        _log = log;
    }

    public async Task<string> SummarizeFileAsync(string filePath)
    {
        var apiKey = _cfg["AzureOpenAI:ApiKey"];
        var endpoint = _cfg["AzureOpenAI:Endpoint"];
        var deployment = _cfg["AzureOpenAI:Deployment"] ?? "gpt-4o-mini";

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(endpoint))
        {
            _log.LogWarning("Azure OpenAI not configured — returning placeholder summary.");
            return "[Placeholder summary] Please configure AzureOpenAI settings to generate real summaries.";
        }

        var client = new OpenAIClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey));

        // For simplicity: extract text from file if it's plain text; for PDF you would hook a PDF text extractor here.
        string content;
        if (Path.GetExtension(filePath).Equals(".txt", StringComparison.OrdinalIgnoreCase))
        {
            content = await File.ReadAllTextAsync(filePath);
        }
        else
        {
            content = "(Document contents extraction not implemented)";
        }

        var prompt = $"Summarize the following document into 3–4 short sentences:\n\n{content}";

        var response = await client.GetCompletionsAsync(deployment, new CompletionsOptions
        {
            Prompts = { prompt },
            MaxTokens = 200
        });

        var text = response.Value.Choices.FirstOrDefault()?.Text?.Trim() ?? string.Empty;
        return text;
    }
}