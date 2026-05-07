using System.Text.Json;
using TripPlanner.Application.Interfaces;
using TripPlanner.Application.Models;
using TripPlanner.Infrastructure.Mapping;
using TripPlanner.Infrastructure.Models.Google;

namespace TripPlanner.Infrastructure.Services.Google
{
    public class TestPlaceProvider : IPlaceProvider
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        public TestPlaceProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public Task<List<PlaceAutoCompleteResult>> AutoCompleteAsync(string query)
        {
            string mockedResponse = File.ReadAllText("D:/Trip-Planner-Data/autocomplete-data.json");
            var result = JsonSerializer.Deserialize<GooglePlaceAutoCompleteResult>(mockedResponse, options);

            return Task.FromResult(result.ToAutoCompleteResults());
        }

        public Task<PlaceResult> GetPlaceAsync(string externalPlaceId)
        {
            string mockedResponse = File.ReadAllText("D:/Trip-Planner-Data/places-data.json");
            var result = JsonSerializer.Deserialize<GooglePlaceTextSearchResult>(mockedResponse, options);
            var place = result.Places.Where(p => p.Id == externalPlaceId).FirstOrDefault() ?? result.Places.ElementAt(Random.Shared.Next(0, result.Places.Count));

            return Task.FromResult(place.ToPlaceResult());
        }

        public Task<string> GetPlacePhotoAsync(string photoName)
        {
            string mockedResponse = File.ReadAllText("D:/Trip-Planner-Data/photos-data.json");
            var result = JsonSerializer.Deserialize<GooglePlacePhotoResult>(mockedResponse, options);

            return Task.FromResult(result.PhotoUri);
        }

        public Task<List<PlaceResult>> NearbySearchPlacesAsync(decimal latitude, decimal longitude)
        {
            string mockedResponse = File.ReadAllText("D:/Trip-Planner-Data/places-data.json");
            var result = JsonSerializer.Deserialize<GooglePlaceTextSearchResult>(mockedResponse, options);

            var returnResult = new List<PlaceResult>();
            foreach (var item in result.Places)
            {
                returnResult.Add(item.ToPlaceResult());
            }

            return Task.FromResult(returnResult);
        }

        public Task<List<PlaceResult>> TextSearchPlacesAsync(decimal latitude, decimal longitude, string query)
        {
            string mockedResponse = File.ReadAllText("D:/Trip-Planner-Data/places-data.json");
            var result = JsonSerializer.Deserialize<GooglePlaceTextSearchResult>(mockedResponse, options);

            var returnResult = new List<PlaceResult>();
            foreach (var item in result.Places)
            {
                returnResult.Add(item.ToPlaceResult());
            }

            return Task.FromResult(returnResult);
        }
    }
}
