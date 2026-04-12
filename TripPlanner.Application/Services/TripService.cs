using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Interfaces;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Services
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IPlaceService _placeService;
        private readonly IUnitOfWork _unitOfWork;
        public TripService(ITripRepository tripRepository, IPlaceService placeService, IUnitOfWork unitOfWork)
        {
            _tripRepository = tripRepository;
            _placeService = placeService;
            _unitOfWork = unitOfWork;
        }

        public async Task AddAsync(TripRequest request, Guid userId)
        {
            var place = await _placeService.GetOrCreateAsync(request.PlaceId);

            if (place == null)
            {
                throw new InvalidOperationException("Place not found");
            }

            await _tripRepository.AddAsync(new Trip
            {
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                PlaceId = place.Id,
                UserId = userId
            });
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<TripResponse>> GetByUserIdAsync(Guid userId)
        {
            var trips = await _tripRepository.GetByUserIdAsync(userId);
            
            return trips.Select(t => new TripResponse
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Place = t.Place,
                CreatedAtUtc = t.CreatedAtUtc
            });
        }
    }
}
