using TripPlanner.Application.Models;

namespace TripPlanner.Application.Interfaces.Providers
{
    public interface IPlaceRecommendationsProvider
    {
        Task<List<PlaceRecommendationResult>> GetPlaceRecommendationsAsync(List<PlaceRecommendationRequest> request, int count, CancellationToken cancellationToken);
    }
}
