using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceRepository
    {
        Task<Place?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Place place, CancellationToken cancellationToken);
        void Remove(Place place);
        Task <IEnumerable<Place>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken);
        Task <IEnumerable<Place>> GetByTripIdAndDayNumberAsync(Guid tripId, int? dayNumber, CancellationToken cancellationToken);
        Task <int> GetMaxOrderForDayAsync(Guid tripId, int? dayNumber, CancellationToken cancellationToken);
    }
}
