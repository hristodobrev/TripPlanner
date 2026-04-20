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

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ReorderAsync(ReorderPlacesRequest request, Guid userId)
        {
            var trip = await _tripRepository.GetByIdForUserAsync(request.TripId!.Value, userId);
            if (trip == null)
            {
                throw new InvalidOperationException("Trip not found");
            }

            var places = await _placeRepository.GetByTripIdAsync(request.TripId!.Value);
            var placesById = places.ToDictionary(p => p.Id);
            foreach (var day in request.Days)
            {
                if (day.DayNumber > trip.DurationInDays)
                {
                    throw new InvalidOperationException($"Day number {day.DayNumber} exceeds trip duration of {trip.DurationInDays} days");
                }
                for (var i = 0; i < day.PlaceIds.Count; i++)
                {
                    var placeId = day.PlaceIds[i];
                    if (!placesById.TryGetValue(placeId, out var place))
                        throw new InvalidOperationException($"Place with ID {placeId} not found in trip");

                    place.DayNumber = day.DayNumber;
                    place.Order = i + 1;
                }
            }

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
    }
}
