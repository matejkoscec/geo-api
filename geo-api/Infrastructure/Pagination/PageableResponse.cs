namespace geo_api.Infrastructure.Pagination;

public record PageableResponse<T>(
    int? CurrentPage,
    int? PageSize,
    List<T> Data
);