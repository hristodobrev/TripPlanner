using TripPlanner.Application.Interfaces.Utilities;

namespace TripPlanner.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
