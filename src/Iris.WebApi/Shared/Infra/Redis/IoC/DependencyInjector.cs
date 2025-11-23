using StackExchange.Redis;

namespace Iris.WebApi.Shared.Infra.Redis.IoC;

public static class DependencyInjector
{
    public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(s =>
        {
            string connection = configuration.GetConnectionString("Redis")!;
            return ConnectionMultiplexer.Connect(connection);
        });
    }
}