namespace TripPlanner.Application.Models
{
    public class PlaceRecommendationRequest
    {
        public string name { get; set; } = null!;
        public string? country { get; set; }
        public string? description { get; set; }
    }
}
