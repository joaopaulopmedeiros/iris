namespace Iris.WebApi.Modules.Indicators.GetStats;

public static class Endpoint
{
    public static WebApplication MapGetIndicatorsStatsEndpoint(this WebApplication app)
    {
        app.MapGet("/indicators/stats", async ([AsParameters] GetIndicatorsStatsRequest request) =>
        {
            await Task.Delay(2);

            IndexStats data = new(
                Average: 400.00m,
                Min: 123.45m,
                Max: 678.90m
            );

            GetIndicatorsStatsResponse response = new(
                Index: request.Index,
                Data: data
            );

            return Results.Ok(response);
        })
        .WithTags("Indicators");

        return app;
    }
}

public record struct GetIndicatorsStatsRequest(string Index, DateOnly From, DateOnly To);

public record struct IndexStats(decimal Average, decimal Min, decimal Max);

public record struct GetIndicatorsStatsResponse(string Index, IndexStats Data);