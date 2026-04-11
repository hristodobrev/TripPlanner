using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceAutocompleteService
    {
        Task<List<PlaceAutocompleteResponse>> AutocompleteAsync(string query);
    }
}
