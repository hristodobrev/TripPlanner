using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceProviderRequestLogRepository
    {
        Task AddAsync(PlaceProviderRequestLog log, CancellationToken cancellationToken);
    }
}
