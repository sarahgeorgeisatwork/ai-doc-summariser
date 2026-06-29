# AI Document Summariser

Minimal ASP.NET Web API that accepts PDF/text uploads and generates short summaries asynchronously using Azure OpenAI.

Features
- ASP.NET Core Web API
- EF Core with SQL Server
- Background worker to generate summaries
- Docker + docker-compose (SQL Server) for local testing
- GitHub Actions workflow to deploy to Azure Web App

Quick start (local)

1. Copy `appsettings.example.json` to `src/appsettings.json` and set connection string and AzureOpenAI settings.

2. Create database and run migrations (use dotnet-ef):

```bash
dotnet tool install --global dotnet-ef
cd src
dotnet ef migrations add InitialCreate
dotnet ef database update
```

3. Run locally:

```bash
cd src
dotnet run
```

Or using Docker Compose:

```bash
docker compose up --build
```

CI/CD
- Set `AZURE_WEBAPP_PUBLISH_PROFILE` and `AZURE_WEBAPP_NAME` secrets in GitHub and push to `main` branch. The workflow builds and deploys to App Service.

Notes / next steps
- Implement robust PDF text extraction (e.g. `PdfPig`, `PdfSharp`) before sending to the OpenAI service.
- Add authentication, file scanning, size limits and storage (Azure Blob Storage) for production.
- Configure retry/backoff and queueing for background processing.
