using geo_api.Infrastructure.Service.Request;
using geo_api.Infrastructure.Service.Response;

namespace geo_api.Infrastructure.Service;

public interface IGooglePlacesApiService
{
    Task<NearbySearchNewResponse> NearbySearch(
        NearbySearchNewRequest request,
        string[] fieldMask,
        CancellationToken cancellationToken);
}