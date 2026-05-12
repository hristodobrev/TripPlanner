using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TripPlanner.Application.Interfaces;
using TripPlanner.Infrastructure.Models.LocalLlm;

namespace TripPlanner.Infrastructure.Services.LocalLlm
{
    public class LocalLlmDescriptionProvider : IDescriptionProvider
    {
        private readonly HttpClient _httpClient;
        public LocalLlmDescriptionProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> GetDescriptionAsync(string placeName, string placeLocation, CancellationToken cancellationToken)
        {
            var body = new
            {
                placeName,
                placeLocation,
                maxLength = 300
            };

            var json = JsonSerializer.Serialize(body);

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "/generate-description")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<LocalLlmGenerateDescriptionResult>();

            return result?.Description;
        }
    }
}
