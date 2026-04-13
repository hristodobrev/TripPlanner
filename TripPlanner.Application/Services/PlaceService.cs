using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Interfaces;
using TripPlanner.Application.Models;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly IPlaceProvider _placeProvider;
        private readonly IUnitOfWork _unitOfWork;

        public PlaceService(IPlaceRepository placeRepository, IPlaceProvider placeProvider, IUnitOfWork unitOfWork)
        {
            _placeRepository = placeRepository;
            _placeProvider = placeProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<Place> GetOrCreateAsync(string externalPlaceId)
        {
            var place = await _placeRepository.GetByExternalId(externalPlaceId);

            if (place != null)
            {
                return place;
            }

            var placeResult = await _placeProvider.GetPlaceAsync(externalPlaceId);
            place = new Place
            {
                ExternalPlaceId = placeResult.Id,
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

            await _placeRepository.AddAsync(place);

            await _unitOfWork.SaveChangesAsync();

            return place;
        }

        public async Task<IEnumerable<PlaceResponse>> TextSearchPlacesAsync(string externalPlaceId, string query)
        {
            var place = await _placeRepository.GetByExternalId(externalPlaceId);

            if (place == null)
            {
                throw new InvalidOperationException("Place not found");
            }

            var placesResult = await _placeProvider.TextSearchPlacesAsync(place.Latitude, place.Longitude, query);

            // TODO: Add logic which will insert all places in the DB to reduce the usage of quota

            // TODO: Call only the TextSearch by Id which has unlimited quota and check for each returned Id if already exists in the DB
            // if exists - get from DB, if not - call the regular TextSearch and insert in the DB

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
