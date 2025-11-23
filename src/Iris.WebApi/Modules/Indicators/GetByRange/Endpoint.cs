namespace Iris.WebApi.Modules.Indicators.GetByRange;

public static class Endpoint
{
    public static WebApplication MapGetIndicatorsByRangeEndpoint(this WebApplication app)
    {
        app.MapGet("/indicators", async ([AsParameters] GetIndicatorsByRangeRequest request) =>
        {
            await Task.Delay(2);

            IEnumerable<Index> data =
            [
                new(DateOnly.Parse("2023-01-01"), 123.45m),
                new(DateOnly.Parse("2023-01-02"), 678.90m)
            ];

            GetIndicatorsByRangeResponse response = new(
                Index: request.Index,
                Data: data
            );

            return Results.Ok(response);
        })
        .WithTags("Indicators");

        return app;
    }
}

public record struct GetIndicatorsByRangeRequest(string Index, DateOnly From, DateOnly To);

public record struct Index(DateOnly Date, decimal Value);

public record struct GetIndicatorsByRangeResponse(string Index, IEnumerable<Index> Data);