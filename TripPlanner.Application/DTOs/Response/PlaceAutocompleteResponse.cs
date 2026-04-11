namespace TripPlanner.Application.DTOs.Response
{
    public class PlaceAutocompleteResponse
    {
        public string PlaceId { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
