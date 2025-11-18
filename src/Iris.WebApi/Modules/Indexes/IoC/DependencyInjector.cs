using Iris.WebApi.Modules.Indexes.GetByRange;

namespace Iris.WebApi.Modules.Indexes.IoC;

public static class DependencyInjector
{
    public static WebApplication MapIndexesEndpoints(this WebApplication app)
    {
        app.MapGetIndexesByRangeEndpoint();
        return app;
    }
}