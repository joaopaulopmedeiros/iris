using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

using Refit;

namespace Iris.WebApi.Shared.Infra.Http.Clients.BCB;

public interface IBCBHttpClient
{
    [Get("/dados/serie/bcdata.sgs.11/dados")]
    Task<IEnumerable<Indicator>> GetSelicAsync([Query] IndicatorQueryParams queryParams);

    [Get("/dados/serie/bcdata.sgs.10844/dados")]
    Task<IEnumerable<Indicator>> GetIpcaAsync([Query] IndicatorQueryParams queryParams);
}