using geo_api.Infrastructure.Service.Response;
using Polly;
using Polly.Retry;

namespace geo_api.Infrastructure;

public static class ResiliencePipelines
{
    public const string NearbySearch = "nearby-search";

    public static IServiceCollection AddResiliencePipelines(this IServiceCollection services)
    {
        services.AddResiliencePipeline<string, NearbySearchNewResponse?>(NearbySearch, pipelineBuilder =>
        {
            pipelineBuilder.AddRetry(new RetryStrategyOptions<NearbySearchNewResponse?>
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                Delay = TimeSpan.Zero,
                ShouldHandle = new PredicateBuilder<NearbySearchNewResponse?>().Handle<HttpRequestException>(),
            });
        });

        return services;
    }
}