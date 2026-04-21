using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Interfaces;
using TripPlanner.Application.Models;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IPlaceProvider _placeProvider;
        private readonly IUnitOfWork _unitOfWork;

        public PlaceService(IPlaceRepository placeRepository, ITripRepository tripRepository, IPlaceProvider placeProvider, IUnitOfWork unitOfWork)
        {
            _placeRepository = placeRepository;
            _tripRepository = tripRepository;
            _placeProvider = placeProvider;
            _unitOfWork = unitOfWork;
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

                int targetOrder = targetPlace.Order;
                sourcePlace.Order = targetPlace.Order;
                var placesToReorder = await _placeRepository.GetByOrderAndDayAsync(request.TripId!.Value, sourcePlace.Id, targetPlace.Order, request.DayNumber);
                for (int i = 0; i < placesToReorder.Count(); i++)
                {
                    var currentPlace = placesToReorder.ElementAt(i);
                    var previousPlace = i == 0 ? targetPlace : placesToReorder.ElementAt(i - 1);

                    if (currentPlace.PlannedTime < previousPlace.PlannedTime?.AddMinutes(previousPlace.DurationMinues ?? 0))
                    {
                        currentPlace.PlannedTime = previousPlace.PlannedTime?.AddMinutes(previousPlace.DurationMinues ?? 0);
                    }

                    currentPlace.Order = targetOrder + i + 1;
                }
            }
            else
            {
                sourcePlace.Order = await _placeRepository.GetMaxOrderForDay(request.TripId!.Value, request.DayNumber) + 1;
            }

            sourcePlace.DayNumber = request.DayNumber;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PlaceResult> GetByExternalIdAsync(string externalId)
        {
            var placeResult = await _placeProvider.GetPlaceAsync(externalId);
            return new PlaceResult
            {
                Id = placeResult.Id,
                Name = placeResult.Name,
                Country = placeResult.Country,
                Locality = placeResult.Locality,
                Latitude = placeResult.Latitude,
                Longitude = placeResult.Longitude,
                WebsiteUri = placeResult.WebsiteUri,
                UserRatingCount = placeResult.UserRatingCount,
                Rating = placeResult.Rating,
                PrimaryTypeDisplayName = placeResult.PrimaryTypeDisplayName
            };
        }

        public async Task<IEnumerable<PlaceDetailsResponse>> GetPlacesForTripWithDetailsAsync(Guid tripId)
        {
            var places = await _placeRepository.GetByTripIdAsync(tripId);

            List<PlaceDetailsResponse> placeResponses = new List<PlaceDetailsResponse>();
            foreach (var place in places)
            {
                var placeResult = await _placeProvider.GetPlaceAsync(place.ExternalId!);
                placeResponses.Add(new PlaceDetailsResponse
                {
                    Id = place.Id,
                    Name = placeResult.Name,
                    Note = place.Note,
                    Country = placeResult.Country,
                    Locality = placeResult.Locality,
                    Latitude = placeResult.Latitude,
                    Longitude = placeResult.Longitude,
                    WebsiteUri = placeResult.WebsiteUri,
                    UserRatingCount = placeResult.UserRatingCount,
                    Rating = placeResult.Rating,
                    PrimaryTypeDisplayName = placeResult.PrimaryTypeDisplayName
                });
            }

            return placeResponses;
        }

        public async Task<IEnumerable<PlaceSearchResponse>> TextSearchPlacesAsync(string externalPlaceId, string query)
        {
            var place = await _placeProvider.GetPlaceAsync(externalPlaceId);

            if (place == null)
            {
                throw new InvalidOperationException("Place not found");
            }

            var placesResult = await _placeProvider.TextSearchPlacesAsync(place.Latitude, place.Longitude, query);

            var places = new List<PlaceSearchResponse>();
            foreach (var placeResult in placesResult)
            {
                places.Add(new PlaceSearchResponse
                {
                    Latitude = placeResult.Latitude,
                    Longitude = placeResult.Longitude,
                    ExternalPlaceId = placeResult.Id,
                    Name = placeResult.Name,
                    Country = placeResult.Country,
                    Locality = placeResult.Locality,
                    WebsiteUri = placeResult.WebsiteUri,
                    UserRatingCount = placeResult.UserRatingCount,
                    Rating = placeResult.Rating,
                    PrimaryTypeDisplayName = placeResult.PrimaryTypeDisplayName
                });
            }

            return places;
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
                PlannedTime = place.PlannedTime
            });
        }
    }
}
