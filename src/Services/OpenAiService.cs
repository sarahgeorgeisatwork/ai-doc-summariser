using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AiDocSummariser.Services;

public class OpenAiService : IOpenAiService
{
    private readonly IConfiguration _cfg;
    private readonly ILogger<OpenAiService> _log;
    private readonly HttpClient _http;

    public OpenAiService(IConfiguration cfg, ILogger<OpenAiService> log, HttpClient http)
    {
        _cfg = cfg;
        _log = log;
        _http = http;
    }

    public async Task<string> SummarizeFileAsync(string filePath)
    {
        var apiKey = _cfg["AzureOpenAI:ApiKey"];
        var endpoint = _cfg["AzureOpenAI:Endpoint"]?.TrimEnd('/');
        var deployment = _cfg["AzureOpenAI:Deployment"] ?? "gpt-4o-mini";
        var apiVersion = _cfg["AzureOpenAI:ApiVersion"] ?? "2023-05-15";

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(endpoint))
        {
            _log.LogWarning("Azure OpenAI not configured — returning placeholder summary.");
            return "[Placeholder summary] Please configure AzureOpenAI settings to generate real summaries.";
        }

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
        var requestUri = $"{endpoint}/openai/deployments/{deployment}/completions?api-version={apiVersion}";

        var requestBody = new
        {
            prompt,
            max_tokens = 200,
            temperature = 0.5,
            n = 1
        };

        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        var json = await JsonDocument.ParseAsync(stream);
        var text = json.RootElement
            .GetProperty("choices")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;

        return text.Trim();
    }
}