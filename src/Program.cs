using Microsoft.EntityFrameworkCore;
using AiDocSummariser.Data;
using AiDocSummariser.Services;
using AiDocSummariser.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton<IOpenAiService, OpenAiService>();
builder.Services.AddHostedService<SummaryBackgroundService>();

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();