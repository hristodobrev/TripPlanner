using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> SearchAsync(string keyword, CancellationToken cancellationToken);
    }
}
