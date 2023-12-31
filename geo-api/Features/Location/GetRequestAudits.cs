﻿using geo_api.Features.Location.Response;
using geo_api.Infrastructure.Pagination;
using geo_api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace geo_api.Features.Location;

public sealed record GetRequestAuditsQuery
    (int? Page, int? PageSize, SortDirection? Sort = SortDirection.Descending) :
        IRequest<PageableResponse<RequestAuditResponse>>, IPageable, ISortable;

public sealed class
    GetRequestAuditsQueryHandler : IRequestHandler<GetRequestAuditsQuery, PageableResponse<RequestAuditResponse>>
{
    private readonly GeoApiContext _dbContext;

    public GetRequestAuditsQueryHandler(GeoApiContext dbContext) { _dbContext = dbContext; }

    public async Task<PageableResponse<RequestAuditResponse>> Handle(
        GetRequestAuditsQuery request,
        CancellationToken cancellationToken)
    {
        var auditsQuery = _dbContext.RequestAudits.AsQueryable();

        if (request.Sort is not null)
        {
            auditsQuery = request.Sort switch
            {
                SortDirection.Ascending => auditsQuery.OrderBy(x => x.CreatedAtUtc),
                SortDirection.Descending => auditsQuery.OrderByDescending(x => x.CreatedAtUtc),
                _ => auditsQuery
            };
        }

        var page = request.Page ?? 1;
        var pageSize = request.PageSize ?? 10;

        var audits = await auditsQuery
            .Skip((page - 1) * (pageSize))
            .Take(pageSize)
            .Select(x => new RequestAuditResponse(
                x.Lat,
                x.Lng,
                x.Radius,
                x.FieldMask,
                x.Request,
                x.Response,
                x.CreatedAtUtc
            ))
            .ToListAsync(cancellationToken);

        return new PageableResponse<RequestAuditResponse>(page, pageSize, audits);
    }
}