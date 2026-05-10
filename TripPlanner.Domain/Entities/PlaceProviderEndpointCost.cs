using TripPlanner.Domain.Enums;

namespace TripPlanner.Domain.Entities
{
    public class PlaceProviderEndpointCost
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public PlaceProvider PlaceProvider { get; set; }
        public PlaceProviderEndpointType EndpointType { get; set; }
        public decimal Cost { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
