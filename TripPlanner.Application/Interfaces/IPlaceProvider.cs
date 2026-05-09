using TripPlanner.Application.Models;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceProvider
    {
        Task<List<PlaceAutoCompleteResult>> AutoCompleteAsync(string query, CancellationToken cancellationToken);
        Task<PlaceResult> GetPlaceAsync(string externalPlaceId, CancellationToken cancellationToken);
        Task<List<PlaceResult>> TextSearchPlacesAsync(decimal latitude, decimal longitude, string query, CancellationToken cancellationToken);
        Task<List<PlaceResult>> NearbySearchPlacesAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken);
        Task<string> GetPlacePhotoAsync(string photoName, CancellationToken cancellationToken);
    }
}
