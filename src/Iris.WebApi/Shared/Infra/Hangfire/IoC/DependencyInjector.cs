using Hangfire.Redis.StackExchange;

namespace Iris.WebApi.Shared.Infra.Hangfire.IoC;

public static class DependencyInjector
{
    public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        RedisStorageOptions storageOptions = new()
        {
            Prefix = "hangfire:"
        };

        services.AddHangfire(config =>
        {
            string connectionString = configuration.GetConnectionString("Redis")!;
            config.UseRedisStorage(connectionString, storageOptions);
        });

        services.AddHangfireServer();

        return services;
    }
}