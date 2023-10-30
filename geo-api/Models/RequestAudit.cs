namespace geo_api.Models;

public class RequestAudit : AuditableEntity
{
    public double Lat { get; set; }

    public double Lng { get; set; }

    public double Radius { get; set; }

    public string FieldMask { get; set; } = null!;

    public string Request { get; set; } = null!;

    public string Response { get; set; } = null!;
}