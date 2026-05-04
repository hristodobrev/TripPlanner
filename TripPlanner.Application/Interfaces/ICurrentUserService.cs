namespace TripPlanner.Application.Interfaces
{
    public interface ICurrentUserService
    {
        public Guid? UserId { get; set; }
    }
}
