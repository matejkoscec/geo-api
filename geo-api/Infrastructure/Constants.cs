using System.Text.Json;

namespace geo_api.Infrastructure;

public static class Json
{
    public static JsonSerializerOptions DefaultSerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}

public static class PollyConstants
{
    public const int RetryCount = 3;

    public static TimeSpan ExponentialBackoff(int retryCount) => TimeSpan.FromMilliseconds(double.Pow(2, retryCount));
}