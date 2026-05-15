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
            await _dbContext.Places.AddAsync(place, cancellationToken);
        }

        public async Task<Place?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken)
        {
            return await _dbContext.Places.Where(p => p.ExternalId == externalId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Place?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Places.Where(p => p.Id == id).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
