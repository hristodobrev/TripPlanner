namespace TripPlanner.Infrastructure.Models.Google
{
    public class GooglePlaceResult
    {
        public string Id { get; set; } = string.Empty;
        public Location Location { get; set; } = null!;
        public LocalizedText DisplayName { get; set; } = null!;
        public List<AddressComponent> AddressComponents { get; set; } = new();
        public double Rating { get; set; }
        public string? WebsiteUri { get; set; }
        public int UserRatingCount { get; set; }
        public LocalizedText? PrimaryTypeDisplayName { get; set; }
    }

    public class Location
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class LocalizedText
    {
        public string Text { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = string.Empty;
    }

    public class AddressComponent
    {
        public string LongText { get; set; } = string.Empty;
        public string ShortText { get; set; } = string.Empty;
        public List<string> Types { get; set; } = new();
        public string LanguageCode { get; set; } = string.Empty;
    }
}
