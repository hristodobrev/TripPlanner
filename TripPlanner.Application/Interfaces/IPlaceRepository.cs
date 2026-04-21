using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceRepository
    {
        Task<Place?> GetByIdAsync(Guid id);
        Task AddAsync(Place place);
        void Remove(Place place);
        Task <IEnumerable<Place>> GetByTripIdAsync(Guid tripId);
        Task <IEnumerable<Place>> GetByOrderAndDayAsync(Guid tripId, Guid placeId, int order, int? dayNumber);
        Task <int> GetMaxOrderForDay(Guid tripId, int? dayNumber);
    }
}
