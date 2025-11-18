namespace Iris.WebApi.Modules.Indexes.GetStats;

public static class Endpoint
{
    public static WebApplication MapGetIndexesStatsEndpoint(this WebApplication app)
    {
        app.MapGet("/indexes/stats", async ([AsParameters] GetIndexesStatsRequest request) =>
        {
            await Task.Delay(2);

            IndexStats data = new(
                Average: 400.00m,
                Min: 123.45m,
                Max: 678.90m
            );

            GetIndexesStatsResponse response = new(
                Index: request.Index,
                Data: data
            );

            return Results.Ok(response);
        })
        .WithTags("Indexes");

        return app;
    }
}

public record struct GetIndexesStatsRequest(string Index, DateOnly From, DateOnly To);

public record struct IndexStats(decimal Average, decimal Min, decimal Max);

public record struct GetIndexesStatsResponse(string Index, IndexStats Data);