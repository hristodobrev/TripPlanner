using TripPlanner.Application.Models;
using TripPlanner.Infrastructure.Models.Google;

namespace TripPlanner.Infrastructure.Mapping
{
    public static class GooglePlaceAutoCompleteMapper
    {
        public static List<PlaceAutoCompleteResult> ToAutoCompleteResults(this GooglePlaceAutoCompleteResult response)
        {
            if (response?.Suggestions == null)
            {
                return new List<PlaceAutoCompleteResult>();
            }

            return response.Suggestions
                .Select(s => new PlaceAutoCompleteResult
                {
                    PlaceId = s.PlacePrediction.PlaceId,
                    MainText = s.PlacePrediction.StructuredFormat?.MainText?.Text ?? string.Empty,
                    SecondaryText = s.PlacePrediction.StructuredFormat?.SecondaryText?.Text ?? string.Empty,
                })
                .ToList();
        }
    }
}
