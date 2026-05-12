using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Infrastructure.Persistence.Configurations
{
    public class PlaceDetailsConfiguration : IEntityTypeConfiguration<PlaceDetails>
    {
        public void Configure(EntityTypeBuilder<PlaceDetails> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ExternalId)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}
