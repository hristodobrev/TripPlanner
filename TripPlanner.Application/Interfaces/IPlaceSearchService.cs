using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceSearchService
    {
        Task<List<PlaceAutoCompleteResponse>> AutoCompleteAsync(string query);
        Task<GetPlaceResponse> GetByExternalIdAsync(string externalId);
        Task<IEnumerable<PlaceTextSearchResponse>> TextSearchPlacesAsync(decimal latitude, decimal longitude, string query);
        Task<IEnumerable<PlaceNearbySearchResponse>> NearbySearchPlacesAsync(decimal latitude, decimal longitude);
        Task<IEnumerable<PlaceDetailsResponse>> GetPlacesForTripWithDetailsAsync(Guid tripId);
    }
}
