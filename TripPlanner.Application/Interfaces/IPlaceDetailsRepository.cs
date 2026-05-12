using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceDetailsRepository
    {
        Task AddAsync(PlaceDetails placeDetails, CancellationToken cancellationToken);
        Task<PlaceDetails?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken);
        Task<PlaceDetails?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
