using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TripPlanner.Application.Interfaces;
using TripPlanner.Application.Models;
using TripPlanner.Infrastructure.Mapping;
using TripPlanner.Infrastructure.Models.Google;

namespace TripPlanner.Infrastructure.Services.Google
{
    public class GooglePlaceAutoCompleteProvider : IPlaceAutoCompleteProvider
    {
        private readonly HttpClient _httpClient;
        public GooglePlaceAutoCompleteProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PlaceAutoCompleteResult>> AutoCompleteAsync(string query)
        {
            var body = new
            {
                input = query,
                includedPrimaryTypes = new string[] { "country", "locality" }
            };

            var json = JsonSerializer.Serialize(body);

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, string.Empty)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GooglePlaceAutoCompleteResult>();

            return result.ToAutoCompleteResults();
        }
    }
}
