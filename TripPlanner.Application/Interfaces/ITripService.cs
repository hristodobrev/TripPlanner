using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.Application.Interfaces
{
    public interface ITripService
    {
        Task<Guid> AddAsync(TripRequest request, Guid userId, CancellationToken cancellationToken);
        Task RemoveAsync(Guid tripId, Guid userId, CancellationToken cancellationToken);
        Task<GetTripResponse> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<GetAllTripResponse>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
