using geo_api.Models;
using Microsoft.EntityFrameworkCore;

namespace geo_api.Infrastructure.Persistence;

public class GeoApiContext : DbContext
{
    public GeoApiContext()
    {
    }

    public GeoApiContext(DbContextOptions<GeoApiContext> options) : base(options)
    {
    }

    public DbSet<RequestAudit> RequestAudits => Set<RequestAudit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RequestAudit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Lat).IsRequired();
            entity.Property(e => e.Lng).IsRequired();
            entity.Property(e => e.Radius).IsRequired();
            entity.Property(e => e.FieldMask).IsRequired();
            entity.Property(e => e.Request).IsRequired();
            entity.Property(e => e.Response).IsRequired();
            entity.Property(e => e.CreatedAtUtc).IsRequired();
        });
    }
}