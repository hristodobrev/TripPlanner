using TripPlanner.Application.Interfaces.Providers;
using TripPlanner.Application.Models;

namespace TripPlanner.API.Tests.Utilities;

public class FakePlaceProvider : IPlaceProvider
{
    public Task<List<PlaceAutoCompleteResult>> AutoCompleteAsync(string query, CancellationToken cancellationToken) =>
        Task.FromResult(new List<PlaceAutoCompleteResult>
        {
            new() { PlaceId = "fake-paris", MainText = "Paris", SecondaryText = "France" }
        });

    public Task<PlaceResult> GetPlaceAsync(string externalPlaceId, CancellationToken cancellationToken) =>
        Task.FromResult(new PlaceResult
        {
            Id = externalPlaceId,
            Name = "Paris",
            FormattedAddress = "Paris, France",
            Country = "France",
            Locality = "Paris",
            Latitude = 48.8566m,
            Longitude = 2.3522m,
            Rating = 4.8,
            UserRatingCount = 1000,
            PrimaryTypeDisplayName = "City",
            Photos = new List<PlacePhotoResult> { new() { Name = "fake-photo" } }
        });

    public Task<List<PlaceResult>> TextSearchPlacesAsync(decimal latitude, decimal longitude, string query, CancellationToken cancellationToken) =>
        Task.FromResult(new List<PlaceResult>
        {
            new()
            {
                Id = "fake-louvre",
                Name = "Louvre Museum",
                FormattedAddress = "Paris, France",
                Country = "France",
                Locality = "Paris",
                Latitude = latitude,
                Longitude = longitude,
                Rating = 4.7,
                UserRatingCount = 5000,
                PrimaryTypeDisplayName = "Museum",
                Photos = new List<PlacePhotoResult> { new() { Name = "fake-photo" } }
            }
        });

    public Task<List<PlaceResult>> NearbySearchPlacesAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken) =>
        Task.FromResult(new List<PlaceResult>
        {
            new()
            {
                Id = "fake-hotel",
                Name = "Test Hotel",
                FormattedAddress = "Paris, France",
                Latitude = latitude,
                Longitude = longitude,
                Rating = 4.5,
                UserRatingCount = 300,
                Photos = new List<PlacePhotoResult>()
            }
        });

    public Task<string> GetPlacePhotoAsync(string photoName, CancellationToken cancellationToken) =>
        Task.FromResult("https://example.com/fake-photo.jpg");
}

public class FakePlaceRecommendationsProvider : IPlaceRecommendationsProvider
{
    public Task<List<PlaceRecommendationResult>> GetPlaceRecommendationsAsync(
        List<PlaceRecommendationRequest> request,
        int count,
        CancellationToken cancellationToken) =>
        Task.FromResult(new List<PlaceRecommendationResult>
        {
            new()
            {
                Name = "Barcelona",
                Country = "Spain",
                Description = "Recommended test destination",
                PlaceId = "fake-barcelona",
                ImageUrl = "https://example.com/barcelona.jpg"
            }
        });
}