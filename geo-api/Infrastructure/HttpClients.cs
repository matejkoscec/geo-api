namespace geo_api.Infrastructure;

public static class HttpClients
{
    public const string GooglePlacesV1 = "google-places-v1";

    public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient(GooglePlacesV1, client =>
        {
            client.BaseAddress = new Uri("https://places.googleapis.com/");
            client.DefaultRequestHeaders.Add("X-Goog-Api-Key", configuration["GoogleApiKeys:Places"]);
        });

        return services;
    }
}