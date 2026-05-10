using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripPlanner.Domain.Entities;
using TripPlanner.Domain.Enums;

namespace TripPlanner.Infrastructure.Persistence.Configurations
{
    public class PlaceProviderEndpointCostConfiguration : IEntityTypeConfiguration<PlaceProviderEndpointCost>
    {
        public void Configure(EntityTypeBuilder<PlaceProviderEndpointCost> builder)
        {
            builder.Property(p => p.PlaceProvider)
                .IsRequired();

            builder.Property(p => p.EndpointType)
                .IsRequired();

            builder.Property(p => p.Cost)
                .HasPrecision(18, 5)
                .IsRequired();

            builder.HasData(
                new PlaceProviderEndpointCost
                {
                    Id = new Guid("24b519c7-3cd5-4e03-984f-97e4946e1b5d"),
                    PlaceProvider = PlaceProvider.GooglePlacesAPI,
                    EndpointType = PlaceProviderEndpointType.AutoCompleteRequests,
                    Cost = 0.00283m,
                    CreatedAtUtc = new DateTime(2026, 5, 10, 10, 29, 50, DateTimeKind.Utc)
                },
                new PlaceProviderEndpointCost
                {
                    Id = new Guid("50511583-e3fd-4997-87c7-745cd25118d2"),
                    PlaceProvider = PlaceProvider.GooglePlacesAPI,
                    EndpointType = PlaceProviderEndpointType.PlaceDetailsEssentialsIdOnly,
                    Cost = 0.0m,
                    CreatedAtUtc = new DateTime(2026, 5, 10, 10, 29, 50, DateTimeKind.Utc)
                },
                new PlaceProviderEndpointCost
                {
                    Id = new Guid("6d6656ed-616e-48c9-b990-f69fa10e58d6"),
                    PlaceProvider = PlaceProvider.GooglePlacesAPI,
                    EndpointType = PlaceProviderEndpointType.PlaceDetailsEssentials,
                    Cost = 0.005m,
                    CreatedAtUtc = new DateTime(2026, 5, 10, 10, 29, 50, DateTimeKind.Utc)
                },
                new PlaceProviderEndpointCost
                {
                    Id = new Guid("734eed77-4170-440b-87a5-49cd0eba1232"),
                    PlaceProvider = PlaceProvider.GooglePlacesAPI,
                    EndpointType = PlaceProviderEndpointType.PlaceDetailsPro,
                    Cost = 0.017m,
                    CreatedAtUtc = new DateTime(2026, 5, 10, 10, 29, 50, DateTimeKind.Utc)
                },
                new PlaceProviderEndpointCost
                {
                    Id = new Guid("80be5d69-4b9a-4325-bc25-98d5a4d1339b"),
                    PlaceProvider = PlaceProvider.GooglePlacesAPI,
                    EndpointType = PlaceProviderEndpointType.PlaceDetailsEnterprise,
                    Cost = 0.02m,
                    CreatedAtUtc = new DateTime(2026, 5, 10, 10, 29, 50, DateTimeKind.Utc)
                },
                new PlaceProviderEndpointCost
                {
                    Id = new Guid("90763803-9b7c-43ea-b2e6-2c1eec863f23"),
                    PlaceProvider = PlaceProvider.GooglePlacesAPI,
                    EndpointType = PlaceProviderEndpointType.PlaceDetailsPhotos,
                    Cost = 0.007m,
                    CreatedAtUtc = new DateTime(2026, 5, 10, 10, 29, 50, DateTimeKind.Utc)
                },
                new PlaceProviderEndpointCost
                {
                    Id = new Guid("9c05f5f2-ad9f-4c23-9a5a-32d84d5bf7d6"),
                    PlaceProvider = PlaceProvider.GooglePlacesAPI,
                    EndpointType = PlaceProviderEndpointType.TextSearchEssentialsIdOnly,
                    Cost = 0.0m,
                    CreatedAtUtc = new DateTime(2026, 5, 10, 10, 29, 50, DateTimeKind.Utc)
                },
                new PlaceProviderEndpointCost
                {
                    Id = new Guid("9ff32d8d-ba07-4cff-abbc-b17a25449cb7"),
                    PlaceProvider = PlaceProvider.GooglePlacesAPI,
                    EndpointType = PlaceProviderEndpointType.TextSearchPro,
                    Cost = 0.032m,
                    CreatedAtUtc = new DateTime(2026, 5, 10, 10, 29, 50, DateTimeKind.Utc)
                },
                new PlaceProviderEndpointCost
                {
                    Id = new Guid("a399866c-d277-46cd-9808-0e84dd0589e5"),
                    PlaceProvider = PlaceProvider.GooglePlacesAPI,
                    EndpointType = PlaceProviderEndpointType.TextSearchEnterprise,
                    Cost = 0.035m,
                    CreatedAtUtc = new DateTime(2026, 5, 10, 10, 29, 50, DateTimeKind.Utc)
                },
                new PlaceProviderEndpointCost
                {
                    Id = new Guid("b20f5d08-fff1-4a64-87d5-82ed2f54628b"),
                    PlaceProvider = PlaceProvider.GooglePlacesAPI,
                    EndpointType = PlaceProviderEndpointType.NearbySearchPro,
                    Cost = 0.032m,
                    CreatedAtUtc = new DateTime(2026, 5, 10, 10, 29, 50, DateTimeKind.Utc)
                },
                new PlaceProviderEndpointCost
                {
                    Id = new Guid("c777bb6d-9f16-473b-8522-d7426b7d30dd"),
                    PlaceProvider = PlaceProvider.GooglePlacesAPI,
                    EndpointType = PlaceProviderEndpointType.NearbySearchEnterprise,
                    Cost = 0.035m,
                    CreatedAtUtc = new DateTime(2026, 5, 10, 10, 29, 50, DateTimeKind.Utc)
                }
            );
        }
    }
}
