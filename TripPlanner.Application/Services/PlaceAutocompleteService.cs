using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Interfaces;

namespace TripPlanner.Application.Services
{
    public class PlaceAutoCompleteService : IPlaceAutoCompleteService
    {
        private readonly IPlaceAutoCompleteProvider _provider;
        public PlaceAutoCompleteService(IPlaceAutoCompleteProvider provider)
        {
            _provider = provider;
        }

        public async Task<List<PlaceAutoCompleteResponse>> AutoCompleteAsync(string query)
        {
            var result = await _provider.AutoCompleteAsync(query);

            return result.Select(r => new PlaceAutoCompleteResponse
            {
                PlaceId = r.PlaceId,
                City = r.City,
                Country = r.Country,
                Description = r.Description
            }).ToList();
        }

        public Task<List<PlaceAutoCompleteResponse>> LocationAutoCompleteAsync(string placeId, string query)
        {


            throw new NotImplementedException();
        }
    }
}
