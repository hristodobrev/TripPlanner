namespace TripPlanner.Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        public Guid? UserId { get; set; }
    }
}
