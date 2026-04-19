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

        public async Task AddAsync(Place place)
        {
            await _dbContext.Places.AddAsync(place);
        }

        public async Task<Place?> GetById(Guid id)
        {
            return await _dbContext.Places.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Place>> GetByTripIdAsync(Guid tripId)
        {
            return await _dbContext.Places.Where(p => p.TripId == tripId).ToListAsync();
        }

        public void Remove(Place place)
        {
            _dbContext.Places.Remove(place);
        }
    }
}
