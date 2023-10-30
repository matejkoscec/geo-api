using System.Text;
using System.Text.Json;
using geo_api.Infrastructure.Service.Request;
using geo_api.Infrastructure.Service.Response;
using Polly;
using Polly.Registry;

namespace geo_api.Infrastructure.Service;

public class GooglePlacesApiNew : IGooglePlacesApiService
{
    private readonly HttpClient _googleApiClient;
    private readonly ResiliencePipeline<NearbySearchNewResponse?> _resiliencePipeline;

    private const string DefaultFieldMask = "places.displayName,places.formattedAddress";

    public GooglePlacesApiNew(IHttpClientFactory httpClientFactory, ResiliencePipelineProvider<string> pipelineProvider)
    {
        _googleApiClient = httpClientFactory.CreateClient(HttpClients.GooglePlacesV1);
        _resiliencePipeline = pipelineProvider.GetPipeline<NearbySearchNewResponse?>(ResiliencePipelines.NearbySearch);
    }

    public async Task<NearbySearchNewResponse?> NearbySearch(
        NearbySearchNewRequest request,
        string[] fieldMask,
        CancellationToken cancellationToken)
    {
        var httpContent = SerializeToJsonContent(request);

        if (fieldMask.Length > 0)
        {
            httpContent.Headers.Add("X-Goog-FieldMask", fieldMask);
        }
        else
        {
            httpContent.Headers.Add("X-Goog-FieldMask", DefaultFieldMask);
        }

        var response = await _resiliencePipeline.ExecuteAsync(
            async token =>
            {
                var response = await _googleApiClient.PostAsync("v1/places:searchNearby", httpContent, token);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<NearbySearchNewResponse>(cancellationToken: token);
            },
            cancellationToken);

        return response;
    }

    private static HttpContent SerializeToJsonContent(object o)
    {
        var jsonContent = JsonSerializer.Serialize(o, Json.DefaultSerializerOptions);

        return new StringContent(jsonContent, Encoding.UTF8, "application/json");
    }
}