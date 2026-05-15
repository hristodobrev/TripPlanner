namespace TripPlanner.Application.Models
{
    public class PlaceRecommendationRequest
    {
        public string Name { get; set; } = null!;
        public string? Country { get; set; }
        public string? Description { get; set; }
    }
}
