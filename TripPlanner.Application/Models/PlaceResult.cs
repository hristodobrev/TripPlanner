namespace TripPlanner.Application.Models
{
    public class PlaceResult
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Country { get; set; }
        public string? Locality { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
