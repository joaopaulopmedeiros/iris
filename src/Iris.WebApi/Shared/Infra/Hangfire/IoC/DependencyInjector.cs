using Hangfire;
using Hangfire.Redis.StackExchange;

namespace Iris.WebApi.Shared.Infra.Hangfire.IoC;

public static class DependencyInjector
{
    public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Redis")!;

        RedisStorageOptions storageOptions = new()
        {
            Prefix = "hangfire:"
        };

        services.AddHangfire(config => config.UseRedisStorage(connectionString, storageOptions));

        services.AddHangfireServer();

        return services;
    }
}