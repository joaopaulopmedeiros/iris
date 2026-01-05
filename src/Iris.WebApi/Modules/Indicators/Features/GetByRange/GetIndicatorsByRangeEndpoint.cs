using System.Diagnostics;

using Iris.WebApi.Modules.Indicators.Features.Ingestion.Models;
using Iris.WebApi.Modules.Indicators.Mappers;
using Iris.WebApi.Modules.Indicators.Models;

using StackExchange.Redis;

namespace Iris.WebApi.Modules.Indicators.Features.GetByRange;

public static class GetIndicatorsByRangeEndpoint
{
    private static readonly ActivitySource ActivitySource = new("Iris.Indicators");
    private const int TotalMillisecondsInADay = 86_400_000;
    public static WebApplication MapGetIndicatorsByRangeEndpoint(this WebApplication app)
    {
        app.MapGet("/indicators", async (
            [AsParameters] GetIndicatorsByRangeRequest request,
            IDatabase redis) =>
        {
            IndicatorConfig? config = IndicatorConfigs.GetByCode(request.Code);

            if (config?.Code is null)
            {
                string validCodes = string.Join(", ", IndicatorConfigs.All.Select(c => c.Code));
                return Results.BadRequest(new { Message = $"Invalid indicator code. Valid codes: {validCodes}" });
            }

            using var redisActivity = ActivitySource.StartActivity("Redis.Query");
            redisActivity?.SetTag("redis.key", config.Value.RedisKey);
            redisActivity?.SetTag("redis.from", request.From.ToString());
            redisActivity?.SetTag("redis.to", request.To.ToString());

            RedisResult timeSeries = await redis.ExecuteAsync(
                "TS.RANGE",
                config.Value.RedisKey,
                request.From.ToUnixMilliseconds(),
                request.To.ToUnixMilliseconds(),
                "AGGREGATION", "last", TotalMillisecondsInADay);

            if (timeSeries.IsNull || timeSeries.Length == 0) return Results.NoContent();

            IEnumerable<Indicator> data = IndicatorMapper.Map((RedisResult[])timeSeries!);
            GetIndicatorsByRangeResponse response = new(request.Code, data);
            return Results.Ok(response);
        })
        .WithTags("Indicators");

        return app;
    }
}