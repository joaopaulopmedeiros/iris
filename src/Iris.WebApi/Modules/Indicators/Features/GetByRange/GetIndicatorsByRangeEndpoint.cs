using Iris.WebApi.Modules.Indicators.Models;

namespace Iris.WebApi.Modules.Indicators.Features.GetByRange;

public static class GetIndicatorsByRangeEndpoint
{
    public static WebApplication MapGetIndicatorsByRangeEndpoint(this WebApplication app)
    {
        app.MapGet("/indicators", async ([AsParameters] GetIndicatorsByRangeRequest request) =>
        {
            await Task.Delay(2);

            IEnumerable<Indicator> data =
            [
                new(DateOnly.Parse("2023-01-01"), 123.45m),
                new(DateOnly.Parse("2023-01-02"), 678.90m)
            ];

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
