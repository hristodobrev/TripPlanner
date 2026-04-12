using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces
{
    public interface ITripRepository
    {
        Task AddAsync(Trip trip);
        Task<Trip?> GetByIdAsync(Guid id);
        Task<IEnumerable<Trip>> GetByUserIdAsync(Guid userId);
    }
}
