using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces
{
    public interface ITripRepository
    {
        Task AddAsync(Trip trip);
        Task<Trip?> GetByIdForUserAsync(Guid id, Guid userId);
        Task<IEnumerable<Trip>> GetByUserIdAsync(Guid userId);
    }
}
