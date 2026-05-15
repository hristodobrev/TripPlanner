using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Exceptions;
using TripPlanner.Application.Interfaces.Utilities;
using TripPlanner.Application.Interfaces.Repositories;
using TripPlanner.Application.Interfaces.Services;
using TripPlanner.Application.Interfaces.Utilities;
using TripPlanner.Application.Models;
using TripPlanner.Domain.Entities;
using TripPlanner.Domain.Enums;

namespace TripPlanner.Application.Services
{
    public class TripPlaceService : ITripPlaceService
    {
        private readonly ITripPlaceRepository _tripPlaceRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public TripPlaceService(ITripPlaceRepository tripPlaceRepository, IPlaceRepository placeRepository, ITripRepository tripRepository, IUnitOfWork unitOfWork, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _tripPlaceRepository = tripPlaceRepository;
            _placeRepository = placeRepository;
            _tripRepository = tripRepository;
            _unitOfWork = unitOfWork;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        public async Task<IEnumerable<TripPlaceResponse>> GetPlacesForTripAsync(Guid tripId, CancellationToken cancellationToken)
        {
            var tripPlaces = await _tripPlaceRepository.GetByTripIdAsync(tripId, cancellationToken);

            return tripPlaces.Select(tripPlace => new TripPlaceResponse
            {
                Id = tripPlace.Id,
                Name = tripPlace.Name,
                Note = tripPlace.Note,
                DayNumber = tripPlace.DayNumber,
                DurationMinutes = tripPlace.DurationMinutes,
                PlannedTime = tripPlace.PlannedTime,
                Status = tripPlace.Status
            });
        }

        public async Task<Guid> AddAsync(AddPlaceRequest request, Guid userId, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdAsync(request.TripId!.Value, cancellationToken);
            CheckAccess(trip, userId);

            Place? place = await _placeRepository.GetByExternalIdAsync(request.ExternalId, cancellationToken);

            if (place == null)
            {
                place = new Place
                {
                    ExternalId = request.ExternalId,
                    Name = request.Name,
                };

                await _placeRepository.AddAsync(place, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            if (place?.Description == null || place?.ImageUrl == null)
            {
                var job = new PlaceDetailsGenerationJob
                {
                    Id = place!.Id,
                    PlaceLocation = trip!.Name,
                    PlaceName = request.Name
                };
                await _backgroundTaskQueue.QueueAsync(job, cancellationToken);
            }

            var tripPlace = new TripPlace
            {
                TripId = request.TripId!.Value,
                Name = request.Name,
                PlaceId = place.Id
            };
            await _tripPlaceRepository.AddAsync(tripPlace, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return tripPlace.Id;
        }

        public async Task RemoveAsync(Guid placeId, Guid userId, CancellationToken cancellationToken)
        {
            var tripPlace = await _tripPlaceRepository.GetByIdAsync(placeId, cancellationToken);
            if (tripPlace == null)
            {
                throw new NotFoundException("Place not found");
            }

            var trip = await _tripRepository.GetByIdAsync(tripPlace.TripId, cancellationToken);
            CheckAccess(trip, userId);

            _tripPlaceRepository.Remove(tripPlace);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Guid id, UpdatePlaceRequest request, Guid userId, CancellationToken cancellationToken)
        {
            var tripPlace = await _tripPlaceRepository.GetByIdAsync(id, cancellationToken);

            if (tripPlace == null)
            {
                throw new NotFoundException("Place not found");
            }

            tripPlace.Note = request.Note;
            tripPlace.DurationMinutes = request.DurationMinutes;
            tripPlace.PlannedTime = request.PlannedTime;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateStatusAsync(Guid id, UpdatePlaceStatusRequest request, Guid userId, CancellationToken cancellationToken)
        {
            var tripPlace = await _tripPlaceRepository.GetByIdAsync(id, cancellationToken);

            if (tripPlace == null)
            {
                throw new NotFoundException("Place not found");
            }

            tripPlace.Status = request.Status;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ReorderAsync(ReorderPlacesRequest request, Guid userId, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdAsync(request.TripId!.Value, cancellationToken);
            CheckAccess(trip, userId);

            var sourcePlace = await _tripPlaceRepository.GetByIdAsync(request.SourceId!.Value, cancellationToken);
            if (sourcePlace == null || sourcePlace.TripId != request.TripId)
            {
                throw new NotFoundException("Place not found in the specified trip");
            }

            TripPlace? targetPlace = null;
            if (request.TargetId != null)
            {
                targetPlace = await _tripPlaceRepository.GetByIdAsync(request.TargetId.Value, cancellationToken);
                if (targetPlace == null || targetPlace.TripId != request.TripId)
                {
                    throw new NotFoundException("Place not found in the specified trip");
                }

                var tripPlaces = await _tripPlaceRepository.GetByTripIdAndDayNumberAsync(request.TripId.Value, targetPlace.DayNumber, cancellationToken);
                int targetOrder = targetPlace.Order;
                if (targetPlace.Order > sourcePlace.Order)
                {
                    foreach (var tripPlace in tripPlaces.Where(p => p.Order <= targetPlace.Order && p.Order >= sourcePlace.Order && p.Id != sourcePlace.Id))
                    {
                        tripPlace.Order -= 1;
                    }
                }
                else
                {
                    foreach (var tripPlace in tripPlaces.Where(p => p.Order >= targetPlace.Order && p.Order <= sourcePlace.Order && p.Id != sourcePlace.Id))
                    {
                        tripPlace.Order += 1;
                    }
                }
                sourcePlace.Order = targetOrder;
                sourcePlace.PlannedTime = targetPlace.PlannedTime ?? sourcePlace.PlannedTime;

                var placesToUpdateTime = tripPlaces.Where(p => p.PlannedTime != null).OrderBy(p => p.Order);
                for (int i = 1; i < placesToUpdateTime.Count(); i++)
                {
                    var currentPlace = placesToUpdateTime.ElementAt(i);
                    var previousPlace = placesToUpdateTime.ElementAt(i - 1);

                    if (currentPlace.PlannedTime < previousPlace.PlannedTime?.AddMinutes(previousPlace.DurationMinutes ?? 0))
                    {
                        currentPlace.PlannedTime = previousPlace.PlannedTime?.AddMinutes(previousPlace.DurationMinutes ?? 0);
                    }
                }
            }
            else if (sourcePlace.DayNumber != request.DayNumber)
            {
                sourcePlace.Order = await _tripPlaceRepository.GetMaxOrderForDayAsync(request.TripId!.Value, request.DayNumber, cancellationToken) + 1;
            }

            sourcePlace.DayNumber = request.DayNumber;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private void CheckAccess(Trip? trip, Guid userId)
        {
            if (trip == null)
            {
                throw new NotFoundException("Trip not found");
            }

            if (trip.UserId != userId && !(trip.TripShares?.Any(t => t.UserId == userId && t.Permission == TripPermission.Edit) ?? false))
            {
                throw new ForbiddenException("Access denied");
            }
        }
    }
}
