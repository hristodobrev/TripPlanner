using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceRecommendationsService
    {
        Task<IEnumerable<PlaceRecommendationsResponse>> GetPlaceRecommendationsAsync(Guid userId, CancellationToken cancellationToken);
    }
}
