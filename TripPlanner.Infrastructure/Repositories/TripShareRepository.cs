
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Interfaces;
using TripPlanner.Domain.Entities;
using TripPlanner.Infrastructure.Persistence;

namespace TripPlanner.Infrastructure.Repositories
{
    public class TripShareRepository : ITripShareRepository
    {
        private readonly AppDbContext _dbContext;

        public TripShareRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TripShare tripShare, CancellationToken cancellationToken)
        {
            await _dbContext.TripShares.AddAsync(tripShare, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await _dbContext.TripShares.Where(ts => ts.Id == id).ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<IEnumerable<TripShare>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken)
        {
            return await _dbContext.TripShares
                .Where(ts => ts.TripId == tripId)
                .Include(ts => ts.User)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TripShare>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.TripShares
                .Where(ts => ts.UserId == userId)
                .Include(ts => ts.Trip)
                .ToListAsync(cancellationToken);
        }

        public void Update(TripShare tripShare)
        {
            _dbContext.TripShares.Update(tripShare);
        }
    }
}
