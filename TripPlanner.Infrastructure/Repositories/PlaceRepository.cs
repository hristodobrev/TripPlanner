using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Interfaces;
using TripPlanner.Domain.Entities;
using TripPlanner.Infrastructure.Persistence;

namespace TripPlanner.Infrastructure.Repositories
{
    public class PlaceRepository : IPlaceRepository
    {
        private readonly AppDbContext _dbContext;

        public PlaceRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Place place, CancellationToken cancellationToken)
        {
            if (place.Order == 0)
            {
                var maxOrder = await _dbContext.Places
                    .Where(p => p.TripId == place.TripId && p.DayNumber == place.DayNumber)
                    .MaxAsync(p => (int?)p.Order, cancellationToken) ?? 0;
                place.Order = maxOrder + 1;
            }

            await _dbContext.Places.AddAsync(place, cancellationToken);
        }

        public async Task<Place?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Places.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Place>> GetByTripIdAndDayNumberAsync(Guid tripId, int? dayNumber, CancellationToken cancellationToken)
        {
            return await _dbContext.Places
                .Where(p => p.TripId == tripId && p.DayNumber == dayNumber)
                .OrderBy(p => p.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Place>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken)
        {
            return await _dbContext.Places
                .Where(p => p.TripId == tripId)
                .Include(p => p.PlaceDetails)
                .OrderBy(p => p.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetMaxOrderForDayAsync(Guid tripId, int? dayNumber, CancellationToken cancellationToken)
        {
            return await _dbContext.Places
                .Where(p => p.TripId == tripId && p.DayNumber == dayNumber)
                .MaxAsync(p => (int?)p.Order, cancellationToken) ?? 0;
        }

        public void Remove(Place place)
        {
            _dbContext.Places.Remove(place);
        }
    }
}
