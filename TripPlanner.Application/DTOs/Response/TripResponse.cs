namespace TripPlanner.Application.DTOs.Response
{
    public class TripResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int DurationInDays => (EndDate - StartDate).Days;

        // TODO: Add PlaceResponse and map it in TripService

        public DateTime CreatedAtUtc { get; set; }
    }
}
