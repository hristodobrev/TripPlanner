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

        public async Task<Guid> AddAsync(TripRequest request, Guid userId)
        {
            var tripToAdd = new Trip
            {
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                DestinationExternalId = request.PlaceId,
                DestinationName = request.PlaceName,
                UserId = userId
            };

            await _tripRepository.AddAsync(tripToAdd);
            await _unitOfWork.SaveChangesAsync();

            return tripToAdd.Id;
        }

        public async Task RemoveAsync(Guid tripId, Guid userId)
        {
            var trip = await _tripRepository.GetByIdForUserAsync(tripId, userId);
            if (trip == null)
            {
                throw new InvalidOperationException("Trip not found");
            }

            _tripRepository.Remove(trip);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<TripResponse> GetByIdForUserAsync(Guid id, Guid userId)
        {
            var trip = await _tripRepository.GetByIdForUserAsync(id, userId);

            if (trip == null)
                throw new InvalidOperationException("Trip not found");

            var places = await _placeService.GetPlacesForTripWithDetailsAsync(id);

            return new TripResponse
            {
                Id = trip.Id,
                Name = trip.Name,
                Description = trip.Description,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                DestinationExternalId = trip.DestinationExternalId,
                Places = places.Select(p => new PlaceDetailsResponse
                {
                    Id = p.Id,
                    ExternalPlaceId = p.ExternalPlaceId,
                    FormattedAddress = p.FormattedAddress,
                    DayNumber = p.DayNumber,
                    Name = p.Name,
                    Note = p.Note,
                    DurationMinutes = p.DurationMinutes,
                    PlannedTime = p.PlannedTime,
                    Locality = p.Locality,
                    Country = p.Country,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    Rating = p.Rating,
                    WebsiteUri = p.WebsiteUri,
                    UserRatingCount = p.UserRatingCount,
                    PrimaryTypeDisplayName = p.PrimaryTypeDisplayName
                }),
                CreatedAtUtc = trip.CreatedAtUtc
            };
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
                DestinationExternalId = t.DestinationExternalId,
                CreatedAtUtc = t.CreatedAtUtc
            });
        }
    }
}
