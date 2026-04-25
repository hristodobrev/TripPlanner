using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceRepository
    {
        Task<Place?> GetByIdAsync(Guid id);
        Task AddAsync(Place place);
        void Remove(Place place);
        Task <IEnumerable<Place>> GetByTripIdAsync(Guid tripId);
        Task <IEnumerable<Place>> GetByTripIdAndDayNumberAsync(Guid tripId, int? dayNumber);
        Task <int> GetMaxOrderForDay(Guid tripId, int? dayNumber);
    }
}
