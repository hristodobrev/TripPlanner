using System.Security;
using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Exceptions;
using TripPlanner.Application.Interfaces;
using TripPlanner.Application.Interfaces.Background;
using TripPlanner.Application.Models;
using TripPlanner.Domain.Entities;
using TripPlanner.Domain.Enums;

namespace TripPlanner.Application.Services
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly ITripShareRepository _tripShareRepository;
        private readonly IPlaceSearchService _placeSearchService;
        private readonly IPlaceRepository _placeRepository;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IUnitOfWork _unitOfWork;
        public TripService(ITripRepository tripRepository, ITripShareRepository tripShareRepository, IPlaceSearchService placeSearchService, IPlaceRepository placeRepository, IBackgroundTaskQueue backgroundTaskQueue, IUnitOfWork unitOfWork)
        {
            _tripRepository = tripRepository;
            _tripShareRepository = tripShareRepository;
            _placeSearchService = placeSearchService;
            _placeRepository = placeRepository;
            _unitOfWork = unitOfWork;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        public async Task<Guid> AddAsync(TripRequest request, Guid userId, CancellationToken cancellationToken)
        {
            var destinationPlace = await _placeRepository.GetByExternalIdAsync(request.PlaceId, cancellationToken);
            if (destinationPlace == null)
            {
                destinationPlace = new Place
                {
                    ExternalId = request.PlaceId,
                    Name = request.DestinationName,
                    Country = request.DestinationCountry
                };
                await _placeRepository.AddAsync(destinationPlace, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var job = new PlaceDetailsGenerationJob
                {
                    Id = destinationPlace!.Id,
                    PlaceLocation = request.DestinationCountry,
                    PlaceName = request.DestinationName
                };
                await _backgroundTaskQueue.QueueAsync(job, cancellationToken);
            }

            var tripToAdd = new Trip
            {
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                DestinationPlaceId = destinationPlace.Id,
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

            var tripLocation = await _placeSearchService.GetByExternalIdAsync(trip.DestinationPlace.ExternalId, cancellationToken);

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
                DestinationExternalId = trip.DestinationPlace.ExternalId,
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
                DestinationExternalId = t.DestinationPlace.ExternalId,
                Shared = shared,
                SharedPermission = permission,
                CreatedAtUtc = t.CreatedAtUtc
            };
        }
    }
}
