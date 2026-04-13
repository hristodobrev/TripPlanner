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

        public async Task<Place?> GetByExternalId(string externalId)
        {
            return await _dbContext.Places.FirstOrDefaultAsync(p => p.ExternalPlaceId == externalId);
        }

        public async Task<bool> CheckAllExternalIdsExist(string[] externalId)
        {
            var existingCount = await _dbContext.Places.CountAsync(p => externalId.Contains(p.ExternalPlaceId));
            return existingCount == externalId.Length;
        }
    }
}
