namespace TripPlanner.Application.Interfaces.Utilities
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
