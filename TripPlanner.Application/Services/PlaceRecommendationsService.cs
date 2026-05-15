using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Interfaces;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Services
{
    public class PlaceRecommendationsService : IPlaceRecommendationsService
    {
        private readonly ITripRepository _tripRepository;
        public PlaceRecommendationsService(ITripRepository tripRepository)
        {
            _tripRepository = tripRepository;
        }

        public async Task<IEnumerable<PlaceRecommendationsResponse>> GetPlaceRecommendationsAsync(Guid userId, CancellationToken cancellationToken)
        {
            IEnumerable<Trip> trips = await _tripRepository.GetByUserIdAsync(userId, cancellationToken);

            if (!trips.Any())
            {
                // TODO: Get top 5 most popular places from the database and use them for the recommendations
            }

            throw new NotImplementedException();
        }
    }
}
