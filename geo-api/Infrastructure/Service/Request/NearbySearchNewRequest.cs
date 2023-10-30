namespace geo_api.Infrastructure.Service.Request;

public sealed record NearbySearchNewRequest(
    LocationRestriction LocationRestriction,
    string[]? IncludedTypes = null,
    int? MaxResultCount = 10,
    RankPreference? RankPreference = null
);

public enum RankPreference
{
    POPULARITY,
    DISTANCE
}

public sealed record LocationRestriction(Circle Circle);

public sealed record Circle(Center Center, double Radius);

public sealed record Center(double Latitude, double Longitude);