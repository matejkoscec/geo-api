namespace geo_api.Features.Location.Response;

public sealed record RequestAuditResponse(
    double Lat,
    double Lng,
    double Radius,
    string FieldMask,
    string Request,
    string Response,
    DateTime CreatedAtUtc
);