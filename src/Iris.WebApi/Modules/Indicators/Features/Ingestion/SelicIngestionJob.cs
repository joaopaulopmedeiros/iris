namespace Iris.WebApi.Modules.Indicators.Features.Ingestion;

public class SelicIngestionJob(ILogger<SelicIngestionJob> logger)
{
    public async Task ExecuteAsync()
    {
        logger.LogInformation("Executing selic ingestion job...");
        await Task.Delay(1000);
    }
}