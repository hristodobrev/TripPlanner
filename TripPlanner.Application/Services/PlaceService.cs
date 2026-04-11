using TripPlanner.Application.Interfaces;
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
            };

            await _placeRepository.AddAsync(place);

            await _unitOfWork.SaveChangesAsync();

            return place;
        }
    }
}
