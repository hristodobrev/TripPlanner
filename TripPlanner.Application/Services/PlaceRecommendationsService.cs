using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Interfaces.Providers;
using TripPlanner.Application.Interfaces.Repositories;
using TripPlanner.Application.Interfaces.Services;
using TripPlanner.Application.Models;
using TripPlanner.Domain.DTOs;

namespace TripPlanner.Application.Services
{
    public class PlaceRecommendationsService : IPlaceRecommendationsService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IPlaceRecommendationsProvider _placeRecommendationsProvider;
        
        public PlaceRecommendationsService(ITripRepository tripRepository, IPlaceRecommendationsProvider placeRecommendationsProvider)
        {
            _tripRepository = tripRepository;
            _placeRecommendationsProvider = placeRecommendationsProvider;
        }

        public async Task<IEnumerable<PlaceRecommendationsResponse>> GetPlaceRecommendationsAsync(Guid userId, CancellationToken cancellationToken)
        {
            IEnumerable<VisitedPlaceDto> visitedPlaces = await _tripRepository.GetTopDestinations(userId, cancellationToken);
            List<PlaceRecommendationRequest> recommendations = new List<PlaceRecommendationRequest>();
            foreach (var item in visitedPlaces)
            {
                recommendations.Add(new PlaceRecommendationRequest
                {
                    name = item.Name,
                    country = item.Country,
                    description = item.Description
                });
            }

            return (await _placeRecommendationsProvider.GetPlaceRecommendationsAsync(recommendations, 3, cancellationToken)).Select(r => new PlaceRecommendationsResponse { 
                Name = r.Name,
                Country = r.Country,
                Description = r.Description,
                PlaceId = r.PlaceId,
                ImageUrl = r.ImageUrl,
                ImageAuthor = r.ImageAuthor,
                ImageAuthorUrl = r.ImageAuthorUrl,
                ImageSource = r.ImageSource,
            });
        }
    }
}
