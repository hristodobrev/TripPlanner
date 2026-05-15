using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Interfaces.Providers;
using TripPlanner.Application.Interfaces.Repositories;
using TripPlanner.Application.Interfaces.Services;

namespace TripPlanner.Application.Services
{
    public class PlaceSearchService : IPlaceSearchService
    {
        private readonly ITripPlaceRepository _placeRepository;
        private readonly IPlaceProvider _placeProvider;

        public PlaceSearchService(IPlaceProvider placeProvider, ITripPlaceRepository placeRepository)
        {
            _placeProvider = placeProvider;
            _placeRepository = placeRepository;
        }

        public async Task<List<PlaceAutoCompleteResponse>> AutoCompleteAsync(string query, CancellationToken cancellationToken)
        {
            var result = await _placeProvider.AutoCompleteAsync(query, cancellationToken);

            return result.Select(r => new PlaceAutoCompleteResponse
            {
                PlaceId = r.PlaceId,
                MainText = r.MainText,
                SecondaryText = r.SecondaryText
            }).ToList();
        }
        public async Task<GetPlaceResponse> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken)
        {
            var placeResult = await _placeProvider.GetPlaceAsync(externalId, cancellationToken);

            // Costs optimization - when getting the place for the trip destination there is no need to get its photos as they aren't used.
            var photoTasks = placeResult.Photos.Take(5).Select(async (p) => await _placeProvider.GetPlacePhotoAsync(p.Name, cancellationToken));
            var photos = await Task.WhenAll(photoTasks);

            return new GetPlaceResponse
            {
                ExternalId = placeResult.Id,
                FormattedAddress = placeResult.FormattedAddress,
                Name = placeResult.Name,
                Country = placeResult.Country,
                Locality = placeResult.Locality,
                Latitude = placeResult.Latitude,    
                Longitude = placeResult.Longitude,
                WebsiteUri = placeResult.WebsiteUri,
                PhotoUrls = photos.ToList(),
                UserRatingCount = placeResult.UserRatingCount,
                Rating = placeResult.Rating,
                PrimaryTypeDisplayName = placeResult.PrimaryTypeDisplayName
            };
        }

        public async Task<IEnumerable<PlacesResponse>> GetPlacesForTripWithDetailsAsync(Guid tripId, CancellationToken cancellationToken)
        {
            var places = await _placeRepository.GetByTripIdAsync(tripId, cancellationToken);

            List<PlacesResponse> placeResponses = new List<PlacesResponse>();
            foreach (var place in places)
            {
                var placeResult = await _placeProvider.GetPlaceAsync(place.Place.ExternalId!, cancellationToken);
                placeResponses.Add(new PlacesResponse
                {
                    Id = place.Id,
                    ExternalPlaceId = placeResult.Id,
                    FormattedAddress = placeResult.FormattedAddress,
                    DayNumber = place.DayNumber,
                    DurationMinutes = place.DurationMinutes,
                    PlannedTime = place.PlannedTime,
                    Name = place.Name, //placeResult.Name,
                    Note = place.Note,
                    Status = place.Status,
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

        public async Task<IEnumerable<PlaceTextSearchResponse>> TextSearchPlacesAsync(decimal latitude, decimal longitude, string query, CancellationToken cancellationToken)
        {
            var placesResult = await _placeProvider.TextSearchPlacesAsync(latitude, longitude, query, cancellationToken);

            var places = new List<PlaceTextSearchResponse>();
            foreach (var placeResult in placesResult)
            {
                places.Add(new PlaceTextSearchResponse
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
                    PrimaryTypeDisplayName = placeResult.PrimaryTypeDisplayName,
                    PhotoUrl = await _placeProvider.GetPlacePhotoAsync(placeResult.Photos.Take(1).Select(p => p.Name).FirstOrDefault(), cancellationToken)
                });
            }

            return places;
        }

        public async Task<IEnumerable<PlaceNearbySearchResponse>> NearbySearchPlacesAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken)
        {
            var placesResult = await _placeProvider.NearbySearchPlacesAsync(latitude, longitude, cancellationToken);

            var places = new List<PlaceNearbySearchResponse>();
            foreach (var placeResult in placesResult)
            {
                places.Add(new PlaceNearbySearchResponse
                {
                    Latitude = placeResult.Latitude,
                    Longitude = placeResult.Longitude,
                    ExternalPlaceId = placeResult.Id,
                    Name = placeResult.Name,
                    UserRatingCount = placeResult.UserRatingCount,
                    Rating = placeResult.Rating,
                });
            }

            return places;
        }
    }
}
