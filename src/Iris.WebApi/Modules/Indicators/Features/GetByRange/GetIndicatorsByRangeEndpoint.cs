using System.Globalization;

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
            string key = $"indicator:{request.Code.ToLower()}";

            IDatabase db = redis.GetDatabase();

            long fromMs = new DateTimeOffset(request.From.ToDateTime(TimeOnly.MinValue)).ToUnixTimeMilliseconds();
            long toMs = new DateTimeOffset(request.To.ToDateTime(TimeOnly.MinValue)).ToUnixTimeMilliseconds();

            RedisResult result = await db.ExecuteAsync("TS.RANGE", key, fromMs, toMs);

            if (result.IsNull) return Results.NoContent();

            IEnumerable<Indicator> data = ((RedisResult[])result!).Select(entry =>
            {
                RedisResult[] parts = (RedisResult[])entry!;

                long timestamp = (long)parts[0];
                DateOnly parsedDate = DateOnly.FromDateTime(
                    DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime
                );

                string value = parts[1].ToString();
                decimal parsedValue = decimal.Parse(value!, NumberStyles.Float, CultureInfo.InvariantCulture);

                return new Indicator(parsedDate, parsedValue);
            });

            GetIndicatorsByRangeResponse response = new(
                Code: request.Code,
                Data: data
            );

            return Results.Ok(response);
        })
        .WithTags("Indicators");

        return app;
    }
}
