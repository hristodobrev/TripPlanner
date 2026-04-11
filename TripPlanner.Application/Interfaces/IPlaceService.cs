using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceService
    {
        public Task<Place> GetOrCreateAsync(string externalPlaceId);
    }
}
