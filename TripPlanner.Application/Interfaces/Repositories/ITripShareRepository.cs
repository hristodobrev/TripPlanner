using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces.Repositories
{
    public interface ITripShareRepository
    {
        Task AddAsync(TripShare tripShare, CancellationToken cancellationToken);
        void Update(TripShare tripShare);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<TripShare?> GetByIdAsync(Guid tripShareId, CancellationToken cancellationToken);
        Task<IEnumerable<TripShare>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken);
        Task<IEnumerable<TripShare>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
