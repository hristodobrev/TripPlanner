using TripPlanner.Application.Models;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceAutoCompleteProvider
    {
        Task<List<PlaceAutoCompleteResult>> AutoCompleteAsync(string query);
    }
}
