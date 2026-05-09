using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Infrastructure.Persistence.Configurations
{
    public class TripShareConfiguration : IEntityTypeConfiguration<TripShare>
    {
        public void Configure(EntityTypeBuilder<TripShare> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasIndex(t => new { t.TripId, t.UserId })
                .IsUnique();

            builder.HasOne(t => t.Trip)
                .WithMany(t => t.TripShares)
                .HasForeignKey(t => t.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(t => t.Permission)
                .IsRequired();
        }
    }
}
