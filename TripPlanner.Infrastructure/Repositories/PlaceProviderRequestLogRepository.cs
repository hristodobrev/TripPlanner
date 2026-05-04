using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Interfaces;
using TripPlanner.Domain.Entities;
using TripPlanner.Infrastructure.Persistence;

namespace TripPlanner.Infrastructure.Repositories
{
    public class PlaceProviderRequestLogRepository : IPlaceProviderRequestLogRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public PlaceProviderRequestLogRepository(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task AddAsync(PlaceProviderRequestLog log)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            await dbContext.PlaceProviderRequestLogs.AddAsync(log);
            await dbContext.SaveChangesAsync();
        }
    }
}
