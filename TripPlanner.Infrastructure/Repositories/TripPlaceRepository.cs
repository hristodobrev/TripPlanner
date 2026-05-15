using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Interfaces;
using TripPlanner.Domain.Entities;
using TripPlanner.Infrastructure.Persistence;

namespace TripPlanner.Infrastructure.Repositories
{
    public class TripPlaceRepository : ITripPlaceRepository
    {
        private readonly AppDbContext _dbContext;

        public TripPlaceRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TripPlace tripPlace, CancellationToken cancellationToken)
        {
            if (tripPlace.Order == 0)
            {
                var maxOrder = await _dbContext.TripPlaces
                    .Where(p => p.TripId == tripPlace.TripId && p.DayNumber == tripPlace.DayNumber)
                    .MaxAsync(p => (int?)p.Order, cancellationToken) ?? 0;
                tripPlace.Order = maxOrder + 1;
            }

            await _dbContext.TripPlaces.AddAsync(tripPlace, cancellationToken);
        }

        public async Task<TripPlace?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.TripPlaces.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<TripPlace>> GetByTripIdAndDayNumberAsync(Guid tripId, int? dayNumber, CancellationToken cancellationToken)
        {
            return await _dbContext.TripPlaces
                .Where(p => p.TripId == tripId && p.DayNumber == dayNumber)
                .OrderBy(p => p.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TripPlace>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken)
        {
            return await _dbContext.TripPlaces
                .Where(p => p.TripId == tripId)
                .Include(p => p.Place)
                .OrderBy(p => p.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetMaxOrderForDayAsync(Guid tripId, int? dayNumber, CancellationToken cancellationToken)
        {
            return await _dbContext.TripPlaces
                .Where(p => p.TripId == tripId && p.DayNumber == dayNumber)
                .MaxAsync(p => (int?)p.Order, cancellationToken) ?? 0;
        }

        public void Remove(TripPlace tripPlace)
        {
            _dbContext.TripPlaces.Remove(tripPlace);
        }
    }
}
