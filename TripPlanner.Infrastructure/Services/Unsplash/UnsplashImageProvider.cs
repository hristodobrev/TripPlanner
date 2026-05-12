using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using TripPlanner.Application.Interfaces;
using TripPlanner.Infrastructure.Models.Unsplash;

namespace TripPlanner.Infrastructure.Services.Unsplash
{
    public class UnsplashImageProvider : IImageProvider
    {
        private readonly HttpClient _httpClient;
        public UnsplashImageProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> GetImageUrlAsync(string placeName, CancellationToken cancellationToken)
        {
            var url = QueryHelpers.AddQueryString(
                "/search/photos",
                new Dictionary<string, string?>
                {
                    ["query"] = placeName,
                    ["orientation"] = "landscape",
                    ["per_page"] = "1"
                });

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<UnsplashSearchPhotoResult>();

            return result?.Results?.FirstOrDefault()?.Urls?.Regular;
        }
    }
}
