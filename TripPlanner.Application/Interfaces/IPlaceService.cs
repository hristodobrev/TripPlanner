using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceService
    {
        Task<IEnumerable<TripPlaceResponse>> GetPlacesForTripAsync(Guid tripId);
        Task<Guid> AddAsync(AddPlaceRequest request, Guid userId);
        Task RemoveAsync(Guid id, Guid userId);
        Task UpdateAsync(Guid id, UpdatePlaceRequest request, Guid userId);
        Task UpdateStatusAsync(Guid id, UpdatePlaceStatusRequest request, Guid userId);
        Task ReorderAsync(ReorderPlacesRequest request, Guid userId);
    }
}
