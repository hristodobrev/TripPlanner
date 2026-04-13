using TripPlanner.Application.DTOs.Response;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceService
    {
        public Task<Place> GetOrCreateAsync(string externalPlaceId);
        public Task<IEnumerable<PlaceResponse>> TextSearchPlacesAsync(string externalPlaceId, string query);
    }
}
