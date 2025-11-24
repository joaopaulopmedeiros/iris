using System.Text.Json;

using StackExchange.Redis;

namespace Iris.WebApi.Shared.Infra.Redis.Extensions;

public static class RedisValuesExtensions
{
    public static IEnumerable<T> ToList<T>(this RedisValue[] entries) where T : class
    {
        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        return entries
            .Where(e => !e.IsNullOrEmpty)
            .Select(e => JsonSerializer.Deserialize<T>((string)e!, options))
            .Where(e => e is not null)!;
    }
}