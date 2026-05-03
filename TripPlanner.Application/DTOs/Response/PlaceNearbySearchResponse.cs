namespace TripPlanner.Application.DTOs.Response
{
    public class PlaceNearbySearchResponse
    {

        public string? ExternalPlaceId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public double Rating { get; set; }
        public int UserRatingCount { get; set; }
    }
}
