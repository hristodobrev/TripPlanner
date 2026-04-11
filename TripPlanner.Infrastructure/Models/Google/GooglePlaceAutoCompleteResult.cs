namespace TripPlanner.Infrastructure.Models.Google
{
    public class GooglePlaceAutoCompleteResult
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
        public StructuredFormat StructuredFormat { get; set; } = null!;
    }

    public class StructuredFormat
    {
        public InnerText MainText { get; set; } = null!;
        public InnerText SecondaryText { get; set; } = null!;
    }

    public class InnerText
    {
        public string Text { get; set; } = string.Empty;
    }
}
