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

        /// <summary>
        /// This Google Places API uses Autocomplete Requests SKU which has 10000 free usage cap and costs $2.83 per 1000 requests (0.00283 per request)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<PlaceAutoCompleteResult>> AutoCompleteAsync(string query)
        {
            var body = new
            {
                input = query,
                includedPrimaryTypes = new string[] { "country", "locality" }
            };

            var json = JsonSerializer.Serialize(body);

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "v1/places:autocomplete")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            string fieldMask = "suggestions.placePrediction.placeId,suggestions.placePrediction.structuredFormat.*";
            requestMessage.Headers.Add("X-Goog-FieldMask", fieldMask);

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GooglePlaceAutoCompleteResult>();

            return result.ToAutoCompleteResults();
        }

        /// <summary>
        /// This Google Places API uses Place Details Enterprise SKU which has 1000 free usage cap and costs $20.00 per 1000 requests (0.02 per request)
        /// </summary>
        /// <param name="externalPlaceId"></param>
        /// <returns></returns>
        public async Task<PlaceResult> GetPlaceAsync(string externalPlaceId)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, $"v1/places/{externalPlaceId}");
            string essentialsIdOnlySkuFields = "id,photos";
            string essentialsFields = "location,addressComponents,formattedAddress";
            string proSkuFields = "displayName,primaryTypeDisplayName";
            string enterpriseSkuFields = "rating,websiteUri,userRatingCount";
            string fieldMask = essentialsIdOnlySkuFields + "," + essentialsFields + "," + proSkuFields + "," + enterpriseSkuFields;
            requestMessage.Headers.Add("X-Goog-FieldMask", fieldMask);

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GooglePlaceResult>();

            return result.ToPlaceResult();
        }

        /// <summary>
        /// This Google Places API uses Place Details Photos SKU which has 1000 free usage cap and costs $7.00 per 1000 requests (0.007 per request)
        /// </summary>
        /// <param name="photoName"></param>
        /// <returns></returns>
        public async Task<string> GetPlacePhotoAsync(string photoName)
        {
            //// Monthly quota for Google Place Photo API of 1000 requests has been reached, so temporarily return null for photo url until the quota is reset in next month
            //return null;

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, $"v1/{photoName}/media?maxWidthPx=800&skipHttpRedirect=true");

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GooglePlacePhotoResult>();

            return result.PhotoUri;
        }

        /// <summary>
        /// This Google Places API uses Text Search Enterprise SKU which has 1000 free usage cap and costs $35.00 per 1000 requests (0.035 per request)
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<PlaceResult>> TextSearchPlacesAsync(decimal latitude, decimal longitude, string query)
        {
            var body = new
            {
                textQuery = query,
                pageSize = 10,
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

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "v1/places:searchText")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            string essentialsIdOnlySkuFields = "places.id";
            string proSkuFields = "places.displayName,places.location,places.primaryTypeDisplayName,places.addressComponents,places.photos";
            string enterpriseSkuFields = "places.priceLevel,places.rating,places.priceRange,places.userRatingCount,places.websiteUri";
            string fieldMask = essentialsIdOnlySkuFields + "," + proSkuFields + "," + enterpriseSkuFields;
            requestMessage.Headers.Add("X-Goog-FieldMask", fieldMask);

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

        /// <summary>
        /// This Google Places API uses Nearby Search Enterprise SKU which has 1000 free usage cap and costs $35.00 per 1000 requests (0.035 per request)
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public async Task<List<PlaceResult>> NearbySearchPlacesAsync(decimal latitude, decimal longitude)
        {
            var body = new
            {
                includedTypes = new string[] { "bed_and_breakfast", "budget_japanese_inn", "campground", "camping_cabin", "cottage", "extended_stay_hotel", "farmstay", "guest_house", "hostel", "hotel", "inn", "japanese_inn", "lodging", "mobile_home_park", "motel", "private_guest_room", "resort_hotel", "rv_park" },
                locationRestriction = new
                {
                    circle = new
                    {
                        center = new { latitude, longitude },
                        radius = 5000
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "v1/places:searchNearby")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            string proSkuFields = "places.id,places.displayName,places.location";
            string enterpriseSkuFields = "places.priceLevel,places.priceRange,places.rating,places.userRatingCount";
            string fieldMask = proSkuFields + "," + enterpriseSkuFields;
            requestMessage.Headers.Add("X-Goog-FieldMask", fieldMask);

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
            string rs = await response.Content.ReadAsStringAsync();
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
