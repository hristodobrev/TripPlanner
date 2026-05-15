using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.Application.Interfaces.Services
{
    public interface IPlaceRecommendationsService
    {
        Task<IEnumerable<PlaceRecommendationsResponse>> GetPlaceRecommendationsAsync(Guid userId, CancellationToken cancellationToken);
    }
}
