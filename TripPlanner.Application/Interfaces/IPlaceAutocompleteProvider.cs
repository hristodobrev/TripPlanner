using TripPlanner.Application.Models;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceAutoCompleteProvider
    {
        Task<List<PlaceAutoCompleteProviderResult>> AutoCompleteAsync(string query);
    }
}
