using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TripPlanner.Application.Interfaces;
using TripPlanner.Application.Models;
using TripPlanner.Infrastructure.Mapping;
using TripPlanner.Infrastructure.Models.Google;

namespace TripPlanner.Infrastructure.Services.Google
{
    public class GooglePlaceProvider : IPlaceProvider
    {
        private readonly HttpClient _httpClient;
        public GooglePlaceProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PlaceResult> GetPlaceAsync(string externalPlaceId)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, externalPlaceId);

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GooglePlaceResult>();

            return result.ToPlaceResult();
        }

        public async Task<List<PlaceResult>> TextSearchPlacesAsync(decimal latitude, decimal longitude, string query)
        {
            var body = new
            {
                textQuery = query,
                locationBias = new
                {
                    circle = new
                    {
                        center = new { latitude, longitude },
                        radius = 5000
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, ":searchText")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            //requestMessage.Headers.Add("X-Goog-FieldMask", "places.id,places.displayName,places.location,places.formattedAddress,places.addressComponents");
            requestMessage.Headers.Add("X-Goog-FieldMask", "places.id,places.displayName,places.formattedAddress,places.priceLevel,places.location,places.rating,places.priceRange,places.priceLevel,places.userRatingCount,places.websiteUri,places.primaryTypeDisplayName,places.addressComponents");

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GooglePlaceTextSearchResult>();

            var returnResult = new List<PlaceResult>();
            foreach (var item in result.Places)
            {
                returnResult.Add(item.ToPlaceResult());
            }

            return returnResult;
        }
    }
}
