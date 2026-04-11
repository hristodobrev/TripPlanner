using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceAutoCompleteService
    {
        Task<List<PlaceAutoCompleteResponse>> AutoCompleteAsync(string query);
        Task<List<PlaceAutoCompleteResponse>> LocationAutoCompleteAsync(string placeId, string query);
    }
}
