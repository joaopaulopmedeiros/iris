using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using StackExchange.Redis;

namespace Iris.WebApi.FunctionalTests.Fixtures;

public sealed class WebApiTestFixture<TEntryPoint> : IAsyncLifetime
    where TEntryPoint : class
{
    private const int RedisPort = 6379;

    private readonly IContainer _redis =
        new ContainerBuilder("redis/redis-stack:7.4.0-v1")
            .WithPortBinding(RedisPort, true)
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilInternalTcpPortIsAvailable(RedisPort))
            .Build();

    public HttpClient HttpClient { get; private set; } = default!;
    public WebApplicationFactory<TEntryPoint> Factory { get; private set; } = default!;

    public async ValueTask InitializeAsync()
    {
        await _redis.StartAsync();

        Factory = CreateFactory();
        HttpClient = Factory.CreateClient();
    }

    public async ValueTask DisposeAsync()
    {
        HttpClient?.Dispose();
        Factory?.Dispose();

        await _redis.StopAsync();
        await _redis.DisposeAsync();
    }

    private WebApplicationFactory<TEntryPoint> CreateFactory()
        => new WebApplicationFactory<TEntryPoint>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");

                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(
                    [
                        new KeyValuePair<string, string?>(
                            "ConnectionStrings:Redis",
                            $"{_redis.Hostname}:{_redis.GetMappedPublicPort(RedisPort)}")
                    ]);
                });

                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IConnectionMultiplexer>();

                    services.AddSingleton<IConnectionMultiplexer>(_ =>
                        ConnectionMultiplexer.Connect(
                            $"{_redis.Hostname}:{_redis.GetMappedPublicPort(RedisPort)}"));
                });
            });
}