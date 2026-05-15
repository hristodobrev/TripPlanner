using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Infrastructure.Persistence.Configurations
{
    public class TripPlaceConfiguration : IEntityTypeConfiguration<TripPlace>
    {
        public void Configure(EntityTypeBuilder<TripPlace> builder)
        {
            builder.ToTable("TripPlaces");

            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Trip)
                .WithMany(t => t.Places)
                .HasForeignKey(p => p.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Place)
                .WithMany()
                .HasForeignKey(p => p.PlaceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(u => u.CreatedAtUtc)
                .IsRequired();
        }
    }
}
