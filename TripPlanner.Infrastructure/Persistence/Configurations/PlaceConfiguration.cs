using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Infrastructure.Persistence.Configurations
{
    public class PlaceConfiguration : IEntityTypeConfiguration<Place>
    {
        public void Configure(EntityTypeBuilder<Place> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(x => x.ExternalPlaceId)
            .IsRequired()
            .HasMaxLength(200);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Country)
                .HasMaxLength(100);

            builder.Property(x => x.Locality)
                .HasMaxLength(100);

            builder.Property(x => x.Latitude)
                .HasPrecision(9, 6);

            builder.Property(x => x.Longitude)
                .HasPrecision(9, 6);

            builder.Property(u => u.CreatedAtUtc)
                .IsRequired();
        }
    }
}
