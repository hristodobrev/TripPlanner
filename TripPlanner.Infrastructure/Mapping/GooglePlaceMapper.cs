using TripPlanner.Application.Models;
using TripPlanner.Infrastructure.Models.Google;

namespace TripPlanner.Infrastructure.Mapping
{
    public static class GooglePlaceMapper
    {
        public static PlaceResult ToPlaceResult(this GooglePlaceResult response)
        {
            var city = GetComponent(response, "locality");
            var country = GetComponent(response, "country");

            return new PlaceResult
            {
                Id = response.Id,
                Name = response.DisplayName?.Text ?? string.Empty,
                FormattedAddress = response.FormattedAddress,
                Country = country,
                Locality = city,
                Latitude = response.Location?.Latitude ?? 0,
                Longitude = response.Location?.Longitude ?? 0,
                Rating = response.Rating,
                WebsiteUri = response.WebsiteUri,
                UserRatingCount = response.UserRatingCount,
                PrimaryTypeDisplayName = response.PrimaryTypeDisplayName?.Text,
                Photos = response.Photos.Select(p => new PlacePhotoResult { Name = p.Name }).ToList()
            };
        }

        private static string? GetComponent(GooglePlaceResult response, string type)
        {
            return response.AddressComponents?
                .FirstOrDefault(c => c.Types.Contains(type))?
                .LongText;
        }
    }
}
