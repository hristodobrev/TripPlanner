using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceRepository
    {
        Task<Place?> GetById(Guid id);
        Task AddAsync(Place place);
        void Remove(Place place);
        Task <IEnumerable<Place>> GetByTripIdAsync(Guid tripId);
    }
}
