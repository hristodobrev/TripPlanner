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

        public async Task AddAsync(Trip trip)
        {
            await _dbContext.Trips.AddAsync(trip);
        }

        public async Task<Trip?> GetByIdForUserAsync(Guid id, Guid userId)
        {
            return await _dbContext.Trips
                .Where(t => t.Id == id && t.UserId == userId)
                .Include(t => t.Places)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Trip>> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.Trips
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public void Remove(Trip trip)
        {
            _dbContext.Trips.Remove(trip);
        }
    }
}
