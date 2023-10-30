using geo_api.Infrastructure.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace geo_api.Features.Location;

public static class LocationEndpoints
{
    public static IEndpointRouteBuilder MapLocationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/locations");

        group.MapGet("/",
            async (int? page, int? pageSize, SortDirection? sort, ISender sender) =>
            {
                var response = await sender.Send(new GetRequestAuditsQuery(page, pageSize, sort));
                return Results.Ok(response);
            });

        group.MapPost("/nearby-search",
            async ([FromBody] NearbySearchQuery query, ISender sender) =>
            {
                var response = await sender.Send(query);
                return Results.Ok(response);
            });

        return app;
    }

    public static IEndpointRouteBuilder MapLocationHubs(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("hub/v1/locations");

        group.MapHub<NearbySearchHub>("nearby-search");

        return app;
    }
}