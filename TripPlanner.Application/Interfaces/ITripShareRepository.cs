using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces
{
    public interface ITripShareRepository
    {
        Task AddAsync(TripShare tripShare, CancellationToken cancellationToken);
        void Update(TripShare tripShare);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<TripShare>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken);
        Task<IEnumerable<TripShare>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
