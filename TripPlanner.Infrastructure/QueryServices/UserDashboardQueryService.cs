using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Interfaces.QueryServices;
using TripPlanner.Domain.DTOs;
using TripPlanner.Domain.Enums;
using TripPlanner.Infrastructure.Persistence;

namespace TripPlanner.Infrastructure.QueryServices
{
    public class UserDashboardQueryService : IUserDashboardQueryService
    {
        private readonly AppDbContext _dbContext;

        public UserDashboardQueryService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserDashboardSummaryDto> GetSummaryAsync(Guid userId, CancellationToken cancellationToken)
        {
            var tripsCount = await _dbContext.Trips
                .CountAsync(t => t.UserId == userId, cancellationToken);

            var visitedPlacesCount = await _dbContext.TripPlaces
                .CountAsync(tp =>
                    tp.Trip.UserId == userId &&
                    tp.Status == PlaceStatus.Visited,
                    cancellationToken);

            var plannedPlacesCount = await _dbContext.TripPlaces
                .CountAsync(tp =>
                    tp.Trip.UserId == userId &&
                    tp.Status == PlaceStatus.Planned,
                    cancellationToken);

            return new UserDashboardSummaryDto
            {
                TripsCount = tripsCount,
                VisitedPlacesCount = visitedPlacesCount,
                PlannedPlacesCount = plannedPlacesCount
            };
        }
    }
}
