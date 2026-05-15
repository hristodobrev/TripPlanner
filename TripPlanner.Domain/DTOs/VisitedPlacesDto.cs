namespace TripPlanner.Domain.DTOs
{
    public class VisitedPlaceDto
    {
        public string Name { get; set; } = null!;
        public string? Country { get; set; }
        public string? Description { get; set; }
    }
}
