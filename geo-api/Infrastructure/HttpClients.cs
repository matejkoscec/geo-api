using geo_api.Infrastructure.Service;
using Polly;

namespace geo_api.Infrastructure;

public static class HttpClients
{
    public const string GooglePlacesV1 = "google-places-v1";

    public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IGooglePlacesApiService, GooglePlacesApiNew>(client =>
            {
                client.BaseAddress = new Uri("https://places.googleapis.com/");
                client.DefaultRequestHeaders.Add("X-Goog-Api-Key", configuration["GoogleApiKeys:Places"]);
            })
            .AddTransientHttpErrorPolicy(policyBuilder =>
                policyBuilder.WaitAndRetryAsync(3, retryCount => TimeSpan.FromMilliseconds(double.Pow(2, retryCount)))
            );

        return services;
    }
}