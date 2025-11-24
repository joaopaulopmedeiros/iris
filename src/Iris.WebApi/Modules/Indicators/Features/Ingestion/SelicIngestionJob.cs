using System.Globalization;

using Iris.WebApi.Shared.Infra.Http.Clients.BCB;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Loggers;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

using Refit;

using StackExchange.Redis;

namespace Iris.WebApi.Modules.Indicators.Features.Ingestion;

[AutomaticRetry(Attempts = 1)]
public class SelicIngestionJob(
    ILogger<SelicIngestionJob> logger,
    IBCBHttpClient httpClient,
    IConnectionMultiplexer redis)
{
    private const string Key = "indicator:selic";

    public async Task ExecuteAsync()
    {
        logger.LogInformation("Executing selic ingestion job...");

        IDatabase db = redis.GetDatabase();

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        DateOnly from = await GetStartDateAsync(db, today);
        DateOnly to = today;

        try
        {
            IEnumerable<Indicator> indicators = await httpClient.GetSelicAsync(new IndicatorQueryParams(from, to));

            if (indicators is null || !indicators.Any())
            {
                logger.LogNoDataFound(from, to);
                return;
            }

            SortedSetEntry[] entries = MapIndicatorsToEntries(indicators);
            await db.SortedSetAddAsync(Key, entries);
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

    private static async Task<DateOnly> GetStartDateAsync(IDatabase db, DateOnly today)
    {
        SortedSetEntry[] lastEntries = await db.SortedSetRangeByRankWithScoresAsync(Key, -1, -1, Order.Ascending);

        if (lastEntries.Length == 0)
        {
            return today.AddYears(-10);
        }

        double lastScore = lastEntries.First().Score;
        DateOnly lastDate = DateOnly.FromDateTime(new DateTime((long)lastScore));

        return lastDate.AddDays(1);
    }

    private static SortedSetEntry[] MapIndicatorsToEntries(IEnumerable<Indicator> indicators)
    {
        return indicators
            .Select(i =>
            {
                long score = i.Date.ToDateTime(TimeOnly.MinValue).Ticks;

                string element = $"{i.Date:yyyyMMdd}:{i.Value.ToString(CultureInfo.InvariantCulture)}";

                return new SortedSetEntry(element, score);
            })
            .ToArray();
    }
}