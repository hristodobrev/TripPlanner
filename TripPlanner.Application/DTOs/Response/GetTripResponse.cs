using TripPlanner.Domain.Enums;

namespace TripPlanner.Application.DTOs.Response
{
    public class GetTripResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DurationInDays => (EndDate - StartDate).Days;
        public string? DestinationExternalId { get; set; }
        public decimal DestinationLatitude { get; set; }
        public decimal DestinationLongitude { get; set; }
        public bool Shared { get; set; }
        public TripPermission? SharedPermission { get; set; }
        public IEnumerable<PlacesResponse> Places { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; }
    }
}
