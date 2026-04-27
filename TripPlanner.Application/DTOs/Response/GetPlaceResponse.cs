namespace TripPlanner.Application.DTOs.Response
{
    public class GetPlaceResponse
    {
        public string ExternalId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string FormattedAddress { get; set; } = null!;
        public string? Country { get; set; }
        public string? Locality { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public double Rating { get; set; }
        public string? WebsiteUri { get; set; }
        public int UserRatingCount { get; set; }
        public string? PrimaryTypeDisplayName { get; set; }
        public List<string> PhotoUrls { get; set; } = new List<string>();
    }
}
