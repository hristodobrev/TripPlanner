namespace TripPlanner.Infrastructure.Models
{
    public class GooglePlaceAutocompleteResult
    {
        public List<Suggestion> Suggestions { get; set; } = new();
    }

    public class Suggestion
    {
        public PlacePrediction PlacePrediction { get; set; } = null!;
    }

    public class PlacePrediction
    {
        public string PlaceId { get; set; } = string.Empty;
        public TextValue Text { get; set; } = null!;
        public StructuredFormat StructuredFormat { get; set; } = null!;
    }

    public class TextValue
    {
        public string Text { get; set; } = string.Empty;
    }

    public class StructuredFormat
    {
        public MainText MainText { get; set; } = null!;
    }

    public class MainText
    {
        public string Text { get; set; } = string.Empty;
    }
}
