using TripPlanner.Application.Models;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceProvider
    {
        Task<List<PlaceAutoCompleteResult>> AutoCompleteAsync(string query);
        Task<PlaceResult> GetPlaceAsync(string externalPlaceId);
        Task<List<PlaceResult>> TextSearchPlacesAsync(decimal latitude, decimal longitude, string query);
        Task<List<PlaceResult>> NearbySearchPlacesAsync(decimal latitude, decimal longitude);
        Task<List<string>> GetPlacePhotosAsync(List<string> photoNames);
        Task<string> GetPlacePhotoAsync(string photoName);
    }
}
