using Refit;

namespace Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

public record struct IndicatorQueryParams(
    DateOnly From,
    DateOnly To,
    string Format = "json"
)
{
    [AliasAs("dataInicial")]
    public readonly string FromBr => From.ToString("dd/MM/yyyy");

    [AliasAs("dataFinal")]
    public readonly string ToBr => To.ToString("dd/MM/yyyy");

    [AliasAs("formato")]
    public readonly string FormatBr => Format;
}