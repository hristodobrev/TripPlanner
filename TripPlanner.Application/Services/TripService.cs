using System.Security;
using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Exceptions;
using TripPlanner.Application.Interfaces;
using TripPlanner.Domain.Entities;
using TripPlanner.Domain.Enums;

namespace TripPlanner.Application.Services
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly ITripShareRepository _tripShareRepository;
        private readonly IPlaceSearchService _placeSearchService;
        private readonly IUnitOfWork _unitOfWork;
        public TripService(ITripRepository tripRepository, ITripShareRepository tripShareRepository, IPlaceSearchService placeSearchService, IUnitOfWork unitOfWork)
        {
            _tripRepository = tripRepository;
            _tripShareRepository = tripShareRepository;
            _placeSearchService = placeSearchService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> AddAsync(TripRequest request, Guid userId, CancellationToken cancellationToken)
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

            await _tripRepository.AddAsync(tripToAdd, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return tripToAdd.Id;
        }

        public async Task RemoveAsync(Guid tripId, Guid userId, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
            if (trip == null)
            {
                throw new NotFoundException("Trip not found");
            }

            if (trip.UserId != userId)
            {
                throw new ForbiddenException("Access denied");
            }

            _tripRepository.Remove(trip);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<GetTripResponse> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdAsync(id, cancellationToken);

            if (trip == null)
                throw new NotFoundException("Trip not found");

            if (trip.UserId != userId && !(trip.TripShares?.Any(ts => ts.UserId == userId) ?? false))
            {
                throw new ForbiddenException("Access denied.");
            }

            var tripLocation = await _placeSearchService.GetByExternalIdAsync(trip.DestinationExternalId, cancellationToken);

            var places = await _placeSearchService.GetPlacesForTripWithDetailsAsync(id, cancellationToken);

            var tripShare = trip.TripShares?.Where(t => t.UserId == userId).FirstOrDefault();

            return new GetTripResponse
            {
                Id = trip.Id,
                Name = trip.Name,
                DestinationLatitude = tripLocation.Latitude,
                DestinationLongitude = tripLocation.Longitude,
                Description = trip.Description,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                DestinationExternalId = trip.DestinationExternalId,
                Places = places,
                Shared = tripShare != null,
                SharedPermission = tripShare?.Permission,
                CreatedAtUtc = trip.CreatedAtUtc
            };
        }

        public async Task<IEnumerable<GetAllTripResponse>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var trips = await _tripRepository.GetByUserIdAsync(userId, cancellationToken);

            var sharedTrips = await _tripShareRepository.GetByUserIdAsync(userId, cancellationToken);

            return trips.Select(t => Map(t, false, null)).Union(sharedTrips.Select(g => Map(g.Trip, true, g.Permission)));
        }

        private GetAllTripResponse Map(Trip t, bool shared, TripPermission? permission)
        {
            return new GetAllTripResponse
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                DestinationExternalId = t.DestinationExternalId,
                Shared = shared,
                SharedPermission = permission,
                CreatedAtUtc = t.CreatedAtUtc
            };
        }
    }
}
