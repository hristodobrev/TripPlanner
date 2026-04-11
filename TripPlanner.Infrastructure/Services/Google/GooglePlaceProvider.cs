using System.Net.Http.Json;
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

        public async Task<PlaceResult> GetPlaceAsync(string placeId)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, placeId);

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GooglePlaceResult>();

            return result.ToPlaceResult();
        }
    }
}
