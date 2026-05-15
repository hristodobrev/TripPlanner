using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TripPlanner.Application.Interfaces.Providers;
using TripPlanner.Application.Models;

namespace TripPlanner.Infrastructure.Services.LocalLlm
{
    public class LocalLlmPlaceRecommendationsProvider : IPlaceRecommendationsProvider
    {
        private readonly HttpClient _httpClient;
        public LocalLlmPlaceRecommendationsProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PlaceRecommendationResult>> GetPlaceRecommendationsAsync(List<PlaceRecommendationRequest> request, int count, CancellationToken cancellationToken)
        {
            var body = new
            {
                visitedPlaces = request,
                count
            };

            var json = JsonSerializer.Serialize(body);

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "/recommend-places")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<PlaceRecommendationResult>>();

            return result;
        }
    }
}
