using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
