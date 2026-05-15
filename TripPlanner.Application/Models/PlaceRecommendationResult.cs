namespace TripPlanner.Application.Models
{
    public class PlaceRecommendationResult
    {
        public string Country { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PlaceId { get; set; } = string.Empty;
        public string ImageAuthor { get; set; } = string.Empty;
        public string ImageAuthorUrl { get; set; } = string.Empty;
        public string ImageSource { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Score { get; set; }
    }
}
