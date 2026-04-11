using TripPlanner.Application.Models;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceProvider
    {
        public Task<PlaceResult> GetPlaceAsync(string placeId);
    }
}
