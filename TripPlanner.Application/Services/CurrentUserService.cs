using TripPlanner.Application.Interfaces;

namespace TripPlanner.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public Guid? UserId { get; set; }
    }
}
