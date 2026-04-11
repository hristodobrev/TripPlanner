using TripPlanner.Application.Models;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceAutocompleteProvider
    {
        Task<List<PlaceAutocompleteProviderResult>> AutocompleteAsync(string query);
    }
}
