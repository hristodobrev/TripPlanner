using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Interfaces;
using TripPlanner.Domain.Entities;
using TripPlanner.Infrastructure.Persistence;

namespace TripPlanner.Infrastructure.Repositories
{
    public class TripRepository : ITripRepository
    {
        private readonly AppDbContext _dbContext;
        public TripRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Trip trip, CancellationToken cancellationToken)
        {
            await _dbContext.Trips.AddAsync(trip, cancellationToken);
        }

        public async Task<Trip?> GetByIdAsync(Guid id,  CancellationToken cancellationToken)
        {
            return await _dbContext.Trips
                .Where(t => t.Id == id)
                .Include(t => t.Places.OrderBy(p => p.Order))
                .Include(t => t.TripShares)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Trip?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Trips
                .Where(t => t.Id == id && t.UserId == userId)
                .Include(t => t.Places.OrderBy(p => p.Order))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<Trip>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Trips
                .Where(t => t.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public void Remove(Trip trip)
        {
            _dbContext.Trips.Remove(trip);
        }
    }
}
