using System.Text.Json;
using FluentValidation;
using geo_api.Infrastructure;
using geo_api.Infrastructure.Persistence;
using geo_api.Infrastructure.Service;
using geo_api.Infrastructure.Service.Request;
using geo_api.Infrastructure.Service.Response;
using geo_api.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace geo_api.Features.Location;

public sealed record NearbySearchQuery(
    double Lat,
    double Lng,
    double Radius,
    string[] FieldMask,
    string[]? IncludedTypes,
    int? MaxResultCount,
    RankPreference? RankPreference
) : IRequest<NearbySearchNewResponse?>;

public sealed class NearbySearchQueryValidator : AbstractValidator<NearbySearchQuery>
{
    public NearbySearchQueryValidator()
    {
        RuleFor(x => x.Lat).NotEmpty().WithMessage("Lat must not be empty").InclusiveBetween(-90, 90);
        RuleFor(x => x.Lng).NotEmpty().WithMessage("Lng must not be empty").InclusiveBetween(-180, 180);
        RuleFor(x => x.Radius).NotEmpty().WithMessage("Radius must not be empty").InclusiveBetween(0, 50000);
        RuleFor(x => x.MaxResultCount).InclusiveBetween(1, 20).WithMessage("MaxResultCount must be between 1 and 20");
        RuleFor(x => x.RankPreference)
            .IsInEnum()
            .WithMessage("RankPreference must be one of the following 0-POPULARITY or 1-DISTANCE");
    }
}

public sealed class NearbySearchRequestHandler : IRequestHandler<NearbySearchQuery, NearbySearchNewResponse?>
{
    private readonly ILogger<NearbySearchRequestHandler> _logger;
    private readonly IGooglePlacesApiService _googlePlaces;
    private readonly GeoApiContext _dbContext;
    private readonly IHubContext<NearbySearchHub, ILocationClient> _hubContext;

    public NearbySearchRequestHandler(
        ILogger<NearbySearchRequestHandler> logger,
        IGooglePlacesApiService googlePlaces,
        GeoApiContext dbContext,
        IHubContext<NearbySearchHub, ILocationClient> hubContext)
    {
        _logger = logger;
        _googlePlaces = googlePlaces;
        _dbContext = dbContext;
        _hubContext = hubContext;
    }

    public async Task<NearbySearchNewResponse?> Handle(NearbySearchQuery query, CancellationToken cancellationToken)
    {
        var request = new NearbySearchNewRequest(
            new LocationRestriction(
                new Circle(
                    new Center(query.Lat, query.Lng),
                    query.Radius)
            ),
            query.IncludedTypes,
            query.MaxResultCount,
            query.RankPreference
        );

        var response = await _googlePlaces.NearbySearch(request, query.FieldMask, cancellationToken);

        var requestAudit = new RequestAudit
        {
            Lat = request.LocationRestriction.Circle.Center.Latitude,
            Lng = request.LocationRestriction.Circle.Center.Longitude,
            Radius = request.LocationRestriction.Circle.Radius,
            FieldMask = string.Join(",", query.FieldMask),
            Request = JsonSerializer.Serialize(request, Json.DefaultSerializerOptions),
            Response = JsonSerializer.Serialize(response, Json.DefaultSerializerOptions)
        };

        await _hubContext.Clients.All.ReceiveMessage(requestAudit.Request);

        _dbContext.RequestAudits.Add(requestAudit);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully saved a request audit. Request: {RequestJson}.", requestAudit.Request);

        return response;
    }
}