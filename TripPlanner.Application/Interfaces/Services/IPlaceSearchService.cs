using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.Application.Interfaces.Services
{
    public interface IPlaceSearchService
    {
        Task<List<PlaceAutoCompleteResponse>> AutoCompleteAsync(string query, CancellationToken cancellationToken);
        Task<GetPlaceResponse> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken);
        Task<IEnumerable<PlaceTextSearchResponse>> TextSearchPlacesAsync(decimal latitude, decimal longitude, string query, CancellationToken cancellationToken);
        Task<IEnumerable<PlaceNearbySearchResponse>> NearbySearchPlacesAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken);
        Task<IEnumerable<PlacesResponse>> GetPlacesForTripWithDetailsAsync(Guid tripId, CancellationToken cancellationToken);
    }
}
