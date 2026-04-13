namespace TripPlanner.Application.DTOs.Response
{
    public class PlaceResponse
    {
        public Guid Id { get; set; }

        public string? ExternalPlaceId { get; set; }
        public string Name { get; set; } = null!;
        public string? Locality { get; set; }
        public string? Country { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public double Rating { get; set; }
        public string? WebsiteUri { get; set; }
        public int UserRatingCount { get; set; }
        public string? PrimaryTypeDisplayName { get; set; }
    }
}
