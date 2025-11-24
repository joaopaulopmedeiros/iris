namespace Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

public record BCBHttpClientOptions
{
    public required string BaseUrl { get; init; }
    public int TimeoutInSeconds { get; init; } = 5;
}