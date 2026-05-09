using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.Application.Interfaces
{
    public interface ITripShareService
    {
        Task AddAsync(Guid tripId, AddTripShareRequest request, Guid userId, CancellationToken cancellationToken);
        Task RemoveAsync(Guid tripId, Guid tripShareId, Guid userId, CancellationToken cancellationToken);
        Task UpdateAsync(Guid tripId, Guid tripShareId, UpdateTripShareRequest request, Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<TripShareResponse>> GetByTripIdAsync(Guid tripId, Guid userId, CancellationToken cancellationToken);
    }
}
