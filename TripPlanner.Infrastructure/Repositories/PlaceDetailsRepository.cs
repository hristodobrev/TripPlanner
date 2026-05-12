using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Interfaces;
using TripPlanner.Domain.Entities;
using TripPlanner.Infrastructure.Persistence;

namespace TripPlanner.Infrastructure.Repositories
{
    public class PlaceDetailsRepository : IPlaceDetailsRepository
    {
        private readonly AppDbContext _dbContext;
        public PlaceDetailsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(PlaceDetails placeDetails, CancellationToken cancellationToken)
        {
            await _dbContext.PlaceDetails.AddAsync(placeDetails, cancellationToken);
        }

        public async Task<PlaceDetails?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken)
        {
            return await _dbContext.PlaceDetails.Where(p => p.ExternalId == externalId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PlaceDetails?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.PlaceDetails.Where(p => p.Id == id).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
