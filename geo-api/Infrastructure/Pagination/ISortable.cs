namespace geo_api.Infrastructure.Pagination;

public interface ISortable
{
    SortDirection? Sort { get; init; }
}