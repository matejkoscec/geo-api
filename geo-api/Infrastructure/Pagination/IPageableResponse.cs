namespace geo_api.Infrastructure.Pagination;

public interface IPageableResponse<T>
{
    int? CurrentPage { get; init; }

    int? PageSize { get; init; }

    List<T> Data { get; init; }
}