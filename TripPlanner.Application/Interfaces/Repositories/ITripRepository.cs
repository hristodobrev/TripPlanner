using TripPlanner.Domain.DTOs;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces.Repositories
{
    public interface ITripRepository
    {
        Task AddAsync(Trip trip, CancellationToken cancellationToken);
        void Remove(Trip trip);
        Task<Trip?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Trip?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<Trip>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<VisitedPlaceDto>> GetTopDestinations(Guid userId, CancellationToken cancellationToken);
    }
}
