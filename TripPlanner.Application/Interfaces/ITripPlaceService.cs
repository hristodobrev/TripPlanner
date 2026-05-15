using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.Application.Interfaces
{
    public interface ITripPlaceService
    {
        Task<IEnumerable<TripPlaceResponse>> GetPlacesForTripAsync(Guid tripId, CancellationToken cancellationToken);
        Task<Guid> AddAsync(AddPlaceRequest request, Guid userId, CancellationToken cancellationToken);
        Task RemoveAsync(Guid id, Guid userId, CancellationToken cancellationToken);
        Task UpdateAsync(Guid id, UpdatePlaceRequest request, Guid userId, CancellationToken cancellationToken);
        Task UpdateStatusAsync(Guid id, UpdatePlaceStatusRequest request, Guid userId, CancellationToken cancellationToken);
        Task ReorderAsync(ReorderPlacesRequest request, Guid userId, CancellationToken cancellationToken);
    }
}
