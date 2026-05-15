using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> SearchAsync(string keyword, CancellationToken cancellationToken);
    }
}
