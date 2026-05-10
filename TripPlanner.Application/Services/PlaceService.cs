using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Exceptions;
using TripPlanner.Application.Interfaces;
using TripPlanner.Domain.Entities;
using TripPlanner.Domain.Enums;

namespace TripPlanner.Application.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PlaceService(IPlaceRepository placeRepository, ITripRepository tripRepository, IUnitOfWork unitOfWork)
        {
            _placeRepository = placeRepository;
            _tripRepository = tripRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TripPlaceResponse>> GetPlacesForTripAsync(Guid tripId, CancellationToken cancellationToken)
        {
            var places = await _placeRepository.GetByTripIdAsync(tripId, cancellationToken);

            return places.Select(place => new TripPlaceResponse
            {
                Id = place.Id,
                Name = place.Name,
                Note = place.Note,
                DayNumber = place.DayNumber,
                DurationMinutes = place.DurationMinues,
                PlannedTime = place.PlannedTime,
                Status = place.Status
            });
        }

        public async Task<Guid> AddAsync(AddPlaceRequest request, Guid userId, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdAsync(request.TripId!.Value, cancellationToken);
            CheckAccess(trip, userId);
            
            var place = new Place
            {
                ExternalId = request.ExternalId,
                TripId = request.TripId!.Value,
                Name = request.Name
            };
            await _placeRepository.AddAsync(place, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return place.Id;
        }

        public async Task RemoveAsync(Guid placeId, Guid userId, CancellationToken cancellationToken)
        {
            var place = await _placeRepository.GetByIdAsync(placeId, cancellationToken);
            if (place == null)
            {
                throw new NotFoundException("Place not found");
            }

            var trip = await _tripRepository.GetByIdAsync(place.TripId, cancellationToken);
            CheckAccess(trip, userId);

            _placeRepository.Remove(place);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Guid id, UpdatePlaceRequest request, Guid userId, CancellationToken cancellationToken)
        {
            var place = await _placeRepository.GetByIdAsync(id, cancellationToken);

            if (place == null)
            {
                throw new NotFoundException("Place not found");
            }

            place.Note = request.Note;
            place.DurationMinues = request.DurationMinutes;
            place.PlannedTime = request.PlannedTime;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateStatusAsync(Guid id, UpdatePlaceStatusRequest request, Guid userId, CancellationToken cancellationToken)
        {
            var place = await _placeRepository.GetByIdAsync(id, cancellationToken);

            if (place == null)
            {
                throw new NotFoundException("Place not found");
            }

            place.Status = request.Status;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ReorderAsync(ReorderPlacesRequest request, Guid userId, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdAsync(request.TripId!.Value, cancellationToken);
            CheckAccess(trip, userId);

            var sourcePlace = await _placeRepository.GetByIdAsync(request.SourceId!.Value, cancellationToken);
            if (sourcePlace == null || sourcePlace.TripId != request.TripId)
            {
                throw new NotFoundException("Place not found in the specified trip");
            }

            Place? targetPlace = null;
            if (request.TargetId != null)
            {
                targetPlace = await _placeRepository.GetByIdAsync(request.TargetId.Value, cancellationToken);
                if (targetPlace == null || targetPlace.TripId != request.TripId)
                {
                    throw new NotFoundException("Place not found in the specified trip");
                }

                var places = await _placeRepository.GetByTripIdAndDayNumberAsync(request.TripId.Value, targetPlace.DayNumber, cancellationToken);
                int targetOrder = targetPlace.Order;
                if (targetPlace.Order > sourcePlace.Order)
                {
                    foreach (var place in places.Where(p => p.Order <= targetPlace.Order && p.Order >= sourcePlace.Order && p.Id != sourcePlace.Id))
                    {
                        place.Order -= 1;
                    }
                }
                else
                {
                    foreach (var place in places.Where(p => p.Order >= targetPlace.Order && p.Order <= sourcePlace.Order && p.Id != sourcePlace.Id))
                    {
                        place.Order += 1;
                    }
                }
                sourcePlace.Order = targetOrder;
                sourcePlace.PlannedTime = targetPlace.PlannedTime ?? sourcePlace.PlannedTime;

                var placesToUpdateTime = places.Where(p => p.PlannedTime != null).OrderBy(p => p.Order);
                for (int i = 1; i < placesToUpdateTime.Count(); i++)
                {
                    var currentPlace = placesToUpdateTime.ElementAt(i);
                    var previousPlace = placesToUpdateTime.ElementAt(i - 1);

                    if (currentPlace.PlannedTime < previousPlace.PlannedTime?.AddMinutes(previousPlace.DurationMinues ?? 0))
                    {
                        currentPlace.PlannedTime = previousPlace.PlannedTime?.AddMinutes(previousPlace.DurationMinues ?? 0);
                    }
                }
            }
            else if (sourcePlace.DayNumber != request.DayNumber)
            {
                sourcePlace.Order = await _placeRepository.GetMaxOrderForDayAsync(request.TripId!.Value, request.DayNumber, cancellationToken) + 1;
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
