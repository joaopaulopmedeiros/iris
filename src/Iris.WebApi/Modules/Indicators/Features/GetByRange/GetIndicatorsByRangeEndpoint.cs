using System.Globalization;

using Iris.WebApi.Modules.Indicators.Models;
using Iris.WebApi.Shared.Infra.Redis.Extensions;

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

            SortedSetEntry[] entries = await redis.GetDatabase()
                .GetSortedSetRangeByScoreAsync(key, request.From, request.To);

            if (entries is null || entries.Length == 0)
                return Results.NoContent();

            IEnumerable<Indicator> data = entries.Select(e =>
            {
                string[] parts = e.Element.ToString().Split(':');
                DateOnly date = DateOnly.ParseExact(parts[0], "yyyyMMdd", CultureInfo.InvariantCulture);
                decimal value = decimal.Parse(parts[1], CultureInfo.InvariantCulture);

                return new Indicator(date, value);
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
