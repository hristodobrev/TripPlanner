using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Interfaces;

namespace TripPlanner.Application.Services
{
    public class PlaceAutoCompleteService : IPlaceAutoCompleteService
    {
        private readonly IPlaceAutoCompleteProvider _provider;
        private readonly IPlaceService _placeService;

        public PlaceAutoCompleteService(IPlaceAutoCompleteProvider provider, IPlaceService placeService)
        {
            _provider = provider;
            _placeService = placeService;
        }

        public async Task<List<PlaceAutoCompleteResponse>> AutoCompleteAsync(string query)
        {
            var result = await _provider.AutoCompleteAsync(query);

            return result.Select(r => new PlaceAutoCompleteResponse
            {
                PlaceId = r.PlaceId,
                MainText = r.MainText,
                SecondaryText = r.SecondaryText
            }).ToList();
        }

        public async Task<List<PlaceAutoCompleteResponse>> LocationAutoCompleteAsync(string externalPlaceId, string query)
        {
            var place = await _placeService.GetByExternalIdAsync(externalPlaceId);

            if (place == null)
            {
                return await AutoCompleteAsync(query);
            }

            var result = await _provider.LocationAutoCompleteAsync(place.Latitude, place.Longitude, query);

            return result.Select(r => new PlaceAutoCompleteResponse
            {
                PlaceId = r.PlaceId,
                MainText = r.MainText,
                SecondaryText = r.SecondaryText
            }).ToList();
        }
    }
}
