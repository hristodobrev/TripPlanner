using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceRepository
    {
        Task AddAsync(Place place, CancellationToken cancellationToken);
        Task<Place?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken);
        Task<Place?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
