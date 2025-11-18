namespace Iris.WebApi.Modules.Indexes.GetByRange;

public static class Endpoint
{
    public static WebApplication MapGetIndexesByRangeEndpoint(this WebApplication app)
    {
        app.MapGet("/indexes", async ([AsParameters] GetIndexesByRangeRequest request) =>
        {
            await Task.Delay(2);

            IEnumerable<Index> data =
            [
                new(DateOnly.Parse("2023-01-01"), 123.45m),
                new(DateOnly.Parse("2023-01-02"), 678.90m)
            ];

            GetIndexesByRangeResponse response = new(
                Index: request.Index,
                Data: data
            );

            return Results.Ok(response);
        })
        .WithTags("Indexes");

        return app;
    }
}

public record struct GetIndexesByRangeRequest(string Index, DateOnly From, DateOnly To);

public record struct Index(DateOnly Date, decimal Value);

public record struct GetIndexesByRangeResponse(string Index, IEnumerable<Index> Data);