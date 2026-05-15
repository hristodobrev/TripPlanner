using TripPlanner.Application.Models;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceRecommendationsProvider
    {
        Task<List<PlaceRecommendationResult>> GetPlaceRecommendationsAsync(List<PlaceRecommendationRequest> request, int count, CancellationToken cancellationToken);
    }
}
