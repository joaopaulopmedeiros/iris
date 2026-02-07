using System.Globalization;

using Iris.WebApi.Modules.Indicators.Features.Ingestion;
using Iris.WebApi.Modules.Indicators.Mappers;
using Iris.WebApi.Modules.Indicators.Models;

using StackExchange.Redis;

namespace Iris.WebApi.Modules.Indicators.Repositories;

public sealed class RedisTimeSeriesRepository(IDatabase redis) : IIndicatorTimeSeriesRepository
{
    public async Task EnsureTimeSeriesExistsAsync(IndicatorConfig config)
    {
        if (await redis.KeyExistsAsync(config.RedisKey))
            return;

        await redis.ExecuteAsync(
            "TS.CREATE",
            config.RedisKey,
            "RETENTION", 0,
            "CHUNK_SIZE", 128,
            "DUPLICATE_POLICY", "LAST",
            "LABELS", "code", config.Code.ToLowerInvariant());
    }

    public async Task<DateOnly> GetNextIngestionDateAsync(IndicatorConfig config, DateOnly today)
    {
        try
        {
            RedisResult result = await redis.ExecuteAsync("TS.GET", config.RedisKey);

            if (result.IsNull)
            {
                return today.AddYears(-10);
            }

            RedisResult[] parts = (RedisResult[])result!;

            if (parts is null || parts.Length == 0)
            {
                return today.AddYears(-10);
            }

            long lastTimestampMs = (long)parts[0];

            DateOnly lastDate = DateOnly.FromDateTime(
                DateTimeOffset.FromUnixTimeMilliseconds(lastTimestampMs).DateTime);

            return lastDate.AddDays(1);
        }
        catch
        {
            return today.AddYears(-10);
        }
    }

    public async Task<IEnumerable<Indicator>> GetIndicatorsAsync(string code, DateOnly from, DateOnly to)
    {
        RedisResult? timeSeries = await redis.ExecuteAsync(
            "TS.RANGE",
            IndicatorConfigs.GetByCode(code).RedisKey,
            from.ToUnixMilliseconds(),
            to.ToUnixMilliseconds());

        if (timeSeries.IsNull || timeSeries.Length == 0)
        {
            return [];
        }

        IEnumerable<Indicator> data = IndicatorMapper.Map((RedisResult[])timeSeries!);

        return data;
    }

    public async Task AppendAsync(IndicatorConfig config, IEnumerable<Indicator> indicators)
    {
        Indicator[] arr = indicators as Indicator[] ?? [.. indicators];

        if (arr.Length == 0)
        {
            return;
        }

        object[] args = new object[arr.Length * 3];

        int index = 0;

        foreach (var indicator in arr)
        {
            long timestampMs = new DateTimeOffset(indicator.Date.ToDateTime(TimeOnly.MinValue))
                .ToUnixTimeMilliseconds();

            args[index++] = config.RedisKey;
            args[index++] = timestampMs;
            args[index++] = indicator.Value.ToString(CultureInfo.InvariantCulture);
        }

        await redis.ExecuteAsync("TS.MADD", args);
    }
}