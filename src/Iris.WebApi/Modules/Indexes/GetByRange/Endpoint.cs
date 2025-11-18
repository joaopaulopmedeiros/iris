namespace Iris.WebApi.Modules.Indexes.GetByRange;

public static class Endpoint
{
    public static WebApplication MapGetIndexesByRangeEndpoint(this WebApplication app)
    {
        app.MapGet("/indexes", async () =>
        {
            await Task.Delay(2);

            IEnumerable<Index> data =
            [
                new(DateOnly.Parse("2023-01-01"), 123.45m),
                new(DateOnly.Parse("2023-01-02"), 678.90m)
            ];

            return Results.Ok(data);
        })
        .WithTags("Indexes");

        return app;
    }
}

public record struct Index(DateOnly Date, decimal Value);