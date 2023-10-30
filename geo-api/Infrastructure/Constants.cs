using System.Text.Json;

namespace geo_api.Infrastructure;

public static class Json
{
    public static JsonSerializerOptions DefaultSerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}