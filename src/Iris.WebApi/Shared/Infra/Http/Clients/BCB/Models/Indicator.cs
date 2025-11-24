using System.Text.Json.Serialization;

namespace Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

public record Indicator(
    [property: JsonPropertyName("data")] DateOnly Date,
    [property: JsonPropertyName("valor")] decimal Value
);