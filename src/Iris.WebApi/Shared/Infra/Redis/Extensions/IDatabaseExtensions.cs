using StackExchange.Redis;

namespace Iris.WebApi.Shared.Infra.Redis.Extensions;

public static class IDatabaseExtensions
{
    public static async Task<RedisValue[]> GetSortedSetRangeByScoreAsync(
        this IDatabase db,
        string key,
        DateOnly from,
        DateOnly to)
    {
        long start = long.Parse(from.ToString("yyyyMMdd"));
        long end = long.Parse(to.ToString("yyyyMMdd"));
        return await db.SortedSetRangeByScoreAsync(key, start, end);
    }
}