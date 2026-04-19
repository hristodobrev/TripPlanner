using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Models;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceService
    {
        Task AddAsync(AddTripPlaceRequest request, Guid userId);
        Task RemoveAsync(Guid placeId, Guid userId);
        public Task<PlaceResult> GetByExternalIdAsync(string externalPlaceId);
        public Task<IEnumerable<PlaceResponse>> TextSearchPlacesAsync(string externalPlaceId, string query);
        public Task<IEnumerable<PlaceResponse>> GetPlacesForTripWithDetailsAsync(Guid tripId);
        public Task<IEnumerable<PlaceResponse>> GetPlacesForTripAsync(Guid tripId);
    }
}
