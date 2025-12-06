using Iris.WebApi.Modules.Indicators.Features.Ingestion.Models;
using Iris.WebApi.Modules.Indicators.Mappers;
using Iris.WebApi.Modules.Indicators.Models;

using StackExchange.Redis;

namespace Iris.WebApi.Modules.Indicators.Features.GetByRange;

public static class GetIndicatorsByRangeEndpoint
{
    public static WebApplication MapGetIndicatorsByRangeEndpoint(this WebApplication app)
    {
        app.MapGet("/indicators", async (
            [AsParameters] GetIndicatorsByRangeRequest request,
            IConnectionMultiplexer redis) =>
        {
            IndicatorConfig? config = IndicatorConfigs.GetByCode(request.Code);

            if (config?.Code is null)
            {
                string validCodes = string.Join(", ", IndicatorConfigs.All.Select(c => c.Code));
                return Results.BadRequest(new { Message = $"Invalid indicator code. Valid codes: {validCodes}" });
            }

            IDatabase db = redis.GetDatabase();

            RedisResult? timeSeries = await db.ExecuteAsync(
                "TS.RANGE",
                config.Value.RedisKey,
                request.From.ToUnixMilliseconds(),
                request.To.ToUnixMilliseconds());

            if (timeSeries.IsNull) return Results.NoContent();

            IEnumerable<Indicator> data = IndicatorMapper.Map((RedisResult[])timeSeries!);

            GetIndicatorsByRangeResponse response = new(request.Code, data);

            return Results.Ok(response);
        })
        .WithTags("Indicators");

        return app;
    }
}