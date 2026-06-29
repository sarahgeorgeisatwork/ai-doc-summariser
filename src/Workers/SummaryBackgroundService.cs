using AiDocSummariser.Data;
using AiDocSummariser.Models;
using Microsoft.EntityFrameworkCore;

namespace AiDocSummariser.Workers;

public class SummaryBackgroundService : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<SummaryBackgroundService> _log;

    public SummaryBackgroundService(IServiceProvider sp, ILogger<SummaryBackgroundService> log)
    {
        _sp = sp;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("SummaryBackgroundService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var openAi = scope.ServiceProvider.GetRequiredService<AiDocSummariser.Services.IOpenAiService>();

                var pending = await db.Documents
                    .Where(d => !db.Summaries.Any(s => s.DocumentId == d.Id))
                    .OrderBy(d => d.UploadedAt)
                    .Take(5)
                    .ToListAsync(stoppingToken);

                foreach (var doc in pending)
                {
                    _log.LogInformation("Generating summary for document {Id}", doc.Id);
                    var fileAbs = Path.Combine(AppContext.BaseDirectory, doc.Path.Replace('/', Path.DirectorySeparatorChar));
                    var summaryText = await openAi.SummarizeFileAsync(fileAbs);

                    var summary = new Summary
                    {
                        DocumentId = doc.Id,
                        Text = summaryText,
                        CreatedAt = DateTime.UtcNow
                    };

                    db.Summaries.Add(summary);
                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error in background summariser");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}