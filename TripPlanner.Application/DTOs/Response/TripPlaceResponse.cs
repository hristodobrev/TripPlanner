namespace TripPlanner.Application.DTOs.Response
{
    public class TripPlaceResponse
    {
        public Guid Id { get; set; }

        public string? ExternalId { get; set; }
        public string Name { get; set; } = null!;
    }
}
