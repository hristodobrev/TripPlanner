using TripPlanner.Application.Models;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceProvider
    {
        public Task<PlaceResult> GetPlaceAsync(string externalPlaceId);
        public Task<List<PlaceResult>> TextSearchPlacesAsync(decimal latitude, decimal longitude, string query);
        public Task<List<string>> GetPlacePhotosAsync(List<string> photoNames);
    }
}
