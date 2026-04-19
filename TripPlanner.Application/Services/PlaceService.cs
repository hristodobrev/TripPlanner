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

        public async Task AddAsync(AddTripPlaceRequest request, Guid userId)
        {
            var trip = await _tripRepository.GetByIdForUserAsync(request.TripId, userId);
            if (trip == null)
            {
                throw new InvalidOperationException("Trip not found");
            }

            var place = new Place
            {
                ExternalId = request.ExternalPlaceId,
                TripId = request.TripId,
                Name = request.Name
            };
            await _placeRepository.AddAsync(place);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid placeId, Guid userId)
        {
            var place = await _placeRepository.GetById(placeId);
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

        public async Task<PlaceResult> GetByExternalIdAsync(string externalPlaceId)
        {
            var placeResult = await _placeProvider.GetPlaceAsync(externalPlaceId);
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

        public async Task<IEnumerable<PlaceResponse>> GetPlacesForTripAsync(Guid tripId)
        {
            var places = await _placeRepository.GetByTripIdAsync(tripId);

            return places.Select(p => new PlaceResponse
            {
                ExternalPlaceId = p.ExternalId,
                Name = p.Name
            });
        }

        public async Task<IEnumerable<PlaceResponse>> GetPlacesForTripWithDetailsAsync(Guid tripId)
        {
            var places = await _placeRepository.GetByTripIdAsync(tripId);

            List<PlaceResponse> placeResponses = new List<PlaceResponse>();
            foreach (var place in places)
            {
                var placeResult = await _placeProvider.GetPlaceAsync(place.ExternalId!);
                placeResponses.Add(new PlaceResponse
                {
                    Id = place.Id,
                    Name = placeResult.Name,
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

        public async Task<IEnumerable<PlaceResponse>> TextSearchPlacesAsync(string externalPlaceId, string query)
        {
            var place = await _placeProvider.GetPlaceAsync(externalPlaceId);

            if (place == null)
            {
                throw new InvalidOperationException("Place not found");
            }

            var placesResult = await _placeProvider.TextSearchPlacesAsync(place.Latitude, place.Longitude, query);

            var places = new List<PlaceResponse>();
            foreach (var placeResult in placesResult)
            {
                places.Add(new PlaceResponse
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
