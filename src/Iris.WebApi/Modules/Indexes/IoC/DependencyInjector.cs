using Iris.WebApi.Modules.Indexes.GetByRange;
using Iris.WebApi.Modules.Indexes.GetStats;

namespace Iris.WebApi.Modules.Indexes.IoC;

public static class DependencyInjector
{
    public static WebApplication MapIndexesEndpoints(this WebApplication app)
    {
        app.MapGetIndexesStatsEndpoint();
        app.MapGetIndexesByRangeEndpoint();
        return app;
    }
}