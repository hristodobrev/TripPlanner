namespace TripPlanner.Application.DTOs.Request
{
    public class AddTripPlaceRequest
    {
        public Guid TripId { get; set; }
        public string ExternalPlaceId { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
