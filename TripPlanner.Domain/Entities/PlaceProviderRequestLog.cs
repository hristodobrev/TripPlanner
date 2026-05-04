using TripPlanner.Domain.Enums;

namespace TripPlanner.Domain.Entities
{
    public class PlaceProviderRequestLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public PlaceProvider Provider { get; set; }
        public PlaceProviderEndpointType EndpointType { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public int DurationMs { get; set; }
        public bool Succeeded { get; set; }
    }
}
