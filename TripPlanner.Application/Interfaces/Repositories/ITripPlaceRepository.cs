using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces.Repositories
{
    public interface ITripPlaceRepository
    {
        Task<TripPlace?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(TripPlace place, CancellationToken cancellationToken);
        void Remove(TripPlace place);
        Task <IEnumerable<TripPlace>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken);
        Task <IEnumerable<TripPlace>> GetByTripIdAndDayNumberAsync(Guid tripId, int? dayNumber, CancellationToken cancellationToken);
        Task <int> GetMaxOrderForDayAsync(Guid tripId, int? dayNumber, CancellationToken cancellationToken);
    }
}
