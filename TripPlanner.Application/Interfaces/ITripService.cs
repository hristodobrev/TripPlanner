using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.Application.Interfaces
{
    public interface ITripService
    {
        Task AddAsync(TripRequest request, Guid userId);
        Task<IEnumerable<TripResponse>> GetByUserIdAsync(Guid userId);
    }
}
