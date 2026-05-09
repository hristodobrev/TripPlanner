using TripPlanner.Domain.Enums;

namespace TripPlanner.Application.DTOs.Response
{
    public class GetAllTripResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? DestinationExternalId { get; set; }
        public bool Shared { get; set; }
        public TripPermission? SharedPermission { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
