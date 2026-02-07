namespace Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

public record RawIndicator(
    [property: JsonPropertyName("data")] DateOnly Date,
    [property: JsonPropertyName("valor")] decimal Value
);