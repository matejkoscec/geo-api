namespace geo_api.Infrastructure.Pagination;

public interface IPageable
{
    int? Page { get; init; }

    int? PageSize { get; init; }
}