using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
