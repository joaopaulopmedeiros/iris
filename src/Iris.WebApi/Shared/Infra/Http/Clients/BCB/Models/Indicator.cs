using Refit;

namespace Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

public record Indicator(
    [AliasAs("data")] DateOnly Date,
    [AliasAs("valor")] decimal Value
);