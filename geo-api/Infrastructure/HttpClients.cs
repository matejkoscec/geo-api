using geo_api.Infrastructure.Service;
using Polly;

namespace geo_api.Infrastructure;

public static class HttpClients
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IGooglePlacesApiService, GooglePlacesApiNew>(client =>
            {
                client.BaseAddress = new Uri("https://places.googleapis.com/");
                client.DefaultRequestHeaders.Add("X-Goog-Api-Key", configuration["GoogleApiKeys:Places"]);
            })
            .AddTransientHttpErrorPolicy(policyBuilder =>
                policyBuilder.WaitAndRetryAsync(PollyConstants.RetryCount, PollyConstants.ExponentialBackoff)
            );

        return services;
    }
}