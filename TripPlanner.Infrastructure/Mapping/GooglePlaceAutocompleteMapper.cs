using TripPlanner.Application.Models;
using TripPlanner.Infrastructure.Models;

namespace TripPlanner.Infrastructure.Mapping
{
    public static class GooglePlaceAutoCompleteMapper
    {
        public static List<PlaceAutoCompleteProviderResult> ToAutoCompleteResults(this GooglePlaceAutoCompleteResult response)
        {
            if (response?.Suggestions == null)
            {
                return new List<PlaceAutoCompleteProviderResult>();
            }

            return response.Suggestions
                .Select(s =>
                {
                    var prediction = s.PlacePrediction;

                    var fullText = prediction.Text?.Text ?? string.Empty;

                    return new PlaceAutoCompleteProviderResult
                    {
                        PlaceId = prediction.PlaceId,
                        City = prediction.StructuredFormat?.MainText?.Text ?? string.Empty,
                        Country = ExtractCountry(fullText),
                        Description = fullText
                    };
                })
                .ToList();
        }

        private static string ExtractCountry(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var parts = text.Split(',', StringSplitOptions.TrimEntries);

            return parts.Length > 1
                ? parts[^1]
                : string.Empty;
        }
    }
}
