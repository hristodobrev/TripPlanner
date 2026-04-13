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
        public string? ExternalPlaceId { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
