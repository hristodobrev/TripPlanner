using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Infrastructure.Persistence.Configurations
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired();

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.StartDate)
                .IsRequired();

            builder.Property(t => t.EndDate)
                .IsRequired();

            builder.Property(t => t.DestinationExternalId)
                .IsRequired();

            builder.Property(t => t.DestinationName)
                .IsRequired();

            builder.Property(t => t.UserId)
                .IsRequired();

            builder.Property(t => t.CreatedAtUtc)
                .IsRequired();

            builder.HasOne(t => t.User)
                .WithMany(u => u.Trips)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
