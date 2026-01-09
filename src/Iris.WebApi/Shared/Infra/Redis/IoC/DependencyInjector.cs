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

        services.AddSingleton(sp =>
            sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

        services.AddOutputCache(options => options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromMinutes(1))));

        services.AddStackExchangeRedisOutputCache(options =>
        {
            string connection = configuration.GetConnectionString("Redis")!;
            options.Configuration = connection;
            options.InstanceName = "iris:outputcache:";
        });
    }
}