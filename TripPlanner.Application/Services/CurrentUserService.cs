using TripPlanner.Application.Interfaces.Services;

namespace TripPlanner.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public Guid? UserId { get; set; }
    }
}
