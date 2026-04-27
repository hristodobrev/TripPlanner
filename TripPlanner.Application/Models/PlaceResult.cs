namespace TripPlanner.Application.Models
{
    public class PlaceResult
    {
        public string Id { get; set; } = null!;
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
        public List<PlacePhotoResult> Photos { get; set; }
    }

    public class PlacePhotoResult
    {
        public string Name { get; set; }
    }
}
