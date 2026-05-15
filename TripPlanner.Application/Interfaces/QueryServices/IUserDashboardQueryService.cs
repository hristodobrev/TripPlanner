using TripPlanner.Domain.DTOs;

namespace TripPlanner.Application.Interfaces.QueryServices
{
    public interface IUserDashboardQueryService
    {
        Task<UserDashboardSummaryDto> GetSummaryAsync(Guid userId, CancellationToken cancellationToken);
    }
}
