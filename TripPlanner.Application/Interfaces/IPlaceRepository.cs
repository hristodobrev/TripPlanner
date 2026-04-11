using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceRepository
    {
        Task<Place?> GetByExternalId(string externalId);
        Task AddAsync(Place place);
    }
}
