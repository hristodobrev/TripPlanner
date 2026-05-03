using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Interfaces;
using TripPlanner.Domain.Entities;

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

        public async Task<IEnumerable<TripPlaceResponse>> GetPlacesForTripAsync(Guid tripId)
        {
            var places = await _placeRepository.GetByTripIdAsync(tripId);

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

        public async Task<Guid> AddAsync(AddPlaceRequest request, Guid userId)
        {
            var trip = await _tripRepository.GetByIdForUserAsync(request.TripId!.Value, userId);
            if (trip == null)
            {
                throw new InvalidOperationException("Trip not found");
            }

            var place = new Place
            {
                ExternalId = request.ExternalId,
                TripId = request.TripId!.Value,
                Name = request.Name
            };
            await _placeRepository.AddAsync(place);

            await _unitOfWork.SaveChangesAsync();

            return place.Id;
        }

        public async Task RemoveAsync(Guid placeId, Guid userId)
        {
            var place = await _placeRepository.GetByIdAsync(placeId);
            if (place == null)
            {
                throw new InvalidOperationException("Place not found");
            }

            var trip = await _tripRepository.GetByIdForUserAsync(place.TripId, userId);
            if (trip == null)
            {
                throw new InvalidOperationException("Trip not found");
            }

            _placeRepository.Remove(place);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, UpdatePlaceRequest request, Guid userId)
        {
            var place = await _placeRepository.GetByIdAsync(id);

            if (place == null)
            {
                throw new InvalidOperationException("Place not found");
            }

            place.Note = request.Note;
            place.DurationMinues = request.DurationMinutes;
            place.PlannedTime = request.PlannedTime;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(Guid id, UpdatePlaceStatusRequest request, Guid userId)
        {
            var place = await _placeRepository.GetByIdAsync(id);

            if (place == null)
            {
                throw new InvalidOperationException("Place not found");
            }

            place.Status = request.Status;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ReorderAsync(ReorderPlacesRequest request, Guid userId)
        {
            var trip = await _tripRepository.GetByIdForUserAsync(request.TripId!.Value, userId);
            if (trip == null)
            {
                throw new InvalidOperationException("Trip not found");
            }

            var sourcePlace = await _placeRepository.GetByIdAsync(request.SourceId!.Value);
            if (sourcePlace == null || sourcePlace.TripId != request.TripId)
            {
                throw new InvalidOperationException("Place not found in the specified trip");
            }

            Place? targetPlace = null;
            if (request.TargetId != null)
            {
                targetPlace = await _placeRepository.GetByIdAsync(request.TargetId.Value);
                if (targetPlace == null || targetPlace.TripId != request.TripId)
                {
                    throw new InvalidOperationException("Place not found in the specified trip");
                }

                var places = await _placeRepository.GetByTripIdAndDayNumberAsync(request.TripId.Value, targetPlace.DayNumber);
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
                sourcePlace.Order = await _placeRepository.GetMaxOrderForDay(request.TripId!.Value, request.DayNumber) + 1;
            }

            sourcePlace.DayNumber = request.DayNumber;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
