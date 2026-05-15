using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Interfaces.Repositories;
using TripPlanner.Domain.DTOs;
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

        public async Task<Trip?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Trips
                .Where(t => t.Id == id)
                .Include(t => t.Places.OrderBy(p => p.Order))
                .Include(t => t.TripShares)
                .Include(t => t.DestinationPlace)
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
                .Include(t => t.DestinationPlace)
                .Where(t => t.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<VisitedPlaceDto>> GetTopDestinations(Guid userId, CancellationToken cancellationToken)
        {
            var places = await _dbContext.Trips
                .Where(t => t.UserId == userId)
                .Select(t => t.DestinationPlace)
                .Where(p => p != null)
                .GroupBy(p => p!.Id)
                .Select(g => new VisitedPlaceDto
                {
                    Name = g.FirstOrDefault()!.Name ?? string.Empty,
                    Country = g.FirstOrDefault()!.Country,
                    Description = g.FirstOrDefault()!.Description
                })
                .ToListAsync(cancellationToken);

            if (places == null || !places.Any())
            {
                places = await _dbContext.Trips
                    .GroupBy(t => t.DestinationPlaceId)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => new VisitedPlaceDto
                    {
                        Name = g.FirstOrDefault()!.DestinationPlace.Name ?? string.Empty,
                        Country = g.FirstOrDefault()!.DestinationPlace.Country,
                        Description = g.FirstOrDefault()!.DestinationPlace.Description
                    })
                    .ToListAsync(cancellationToken);
            }

            return places;
        }

        public void Remove(Trip trip)
        {
            _dbContext.Trips.Remove(trip);
        }
    }
}
