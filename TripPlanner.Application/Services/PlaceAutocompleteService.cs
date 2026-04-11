using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Interfaces;

namespace TripPlanner.Application.Services
{
    public class PlaceAutocompleteService : IPlaceAutocompleteService
    {
        private readonly IPlaceAutocompleteProvider _provider;
        public PlaceAutocompleteService(IPlaceAutocompleteProvider provider)
        {
            _provider = provider;
        }

        public async Task<List<PlaceAutocompleteResponse>> AutocompleteAsync(string query)
        {
            var result = await _provider.AutocompleteAsync(query);

            return result.Select(r => new PlaceAutocompleteResponse
            {
                PlaceId = r.PlaceId,
                City = r.City,
                Country = r.Country,
                Description = r.Description
            }).ToList();
        }
    }
}
