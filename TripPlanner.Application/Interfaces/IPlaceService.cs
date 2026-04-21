using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Models;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceService
    {
        Task<Guid> AddAsync(AddPlaceRequest request, Guid userId);
        Task RemoveAsync(Guid id, Guid userId);
        Task UpdateAsync(Guid id, UpdatePlaceRequest request, Guid userId);
        Task ReorderAsync(ReorderPlacesRequest request, Guid userId);
        public Task<PlaceResult> GetByExternalIdAsync(string externalId);
        public Task<IEnumerable<PlaceSearchResponse>> TextSearchPlacesAsync(string externalId, string query);
        public Task<IEnumerable<PlaceDetailsResponse>> GetPlacesForTripWithDetailsAsync(Guid tripId);
        public Task<IEnumerable<TripPlaceResponse>> GetPlacesForTripAsync(Guid tripId);
    }
}
