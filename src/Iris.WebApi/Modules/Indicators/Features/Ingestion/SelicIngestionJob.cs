using Iris.WebApi.Shared.Infra.Http.Clients.BCB;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Logs;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

using Refit;

namespace Iris.WebApi.Modules.Indicators.Features.Ingestion;

public class SelicIngestionJob(ILogger<SelicIngestionJob> logger, IBCBHttpClient httpClient)
{
    public async Task ExecuteAsync()
    {
        logger.LogInformation("Executing selic ingestion job...");

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        DateOnly from = today;
        DateOnly to = today;

        try
        {
            IEnumerable<Indicator> indicators = await httpClient.GetSelicAsync(new IndicatorQueryParams(from, to));

            if (indicators is null || !indicators.Any())
            {
                logger.LogInformation("No selic indicator values found for today");
                return;
            }
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            logger.LogNoDataFound(from, to);
        }
        catch (ApiException ex)
        {
            logger.LogUnhandledException((int)ex.StatusCode, ex.Content ?? string.Empty);
            throw;
        }
    }
}