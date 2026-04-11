namespace TripPlanner.Infrastructure.Models.Google
{
    public class GooglePlaceResult
    {
        public string Id { get; set; } = string.Empty;
        public Location Location { get; set; } = null!;
        public DisplayName DisplayName { get; set; } = null!;
        public List<AddressComponent> AddressComponents { get; set; } = new();
    }

    public class Location
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class DisplayName
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
